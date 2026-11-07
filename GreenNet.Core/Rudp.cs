using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

namespace GreenNet.Core;

/// <summary>
/// UDP tin cậy tối giản (selective repeat, cửa sổ 64 gói) để truyền luồng byte qua UDP.
/// Gói: DATA/FIN = [loại 1B][session 8B][seq 4B][payload ≤1200B]; ACK = [3][session 8B][cum 4B][bitmap 8B].
/// Không có kiểm soát tắc nghẽn: chỉ phù hợp mạng LAN/thí nghiệm, đúng mục đích so sánh với TCP.
/// </summary>
internal static class RudpConst
{
    public const byte Data = 1, Fin = 2, Ack = 3;
    public const int Payload = 1200;
    public const int Window = 64;
    public const int HeaderLen = 13;
}

/// <summary>Phía gửi: là một Stream chỉ ghi. Gọi CompleteAsync() khi gửi xong.</summary>
public sealed class RudpSender : Stream
{
    private sealed class Pkt
    {
        public uint Seq;
        public byte[] Wire;
        public long SentAt;
        public int Tries;
        public bool Acked;
    }

    private readonly UdpClient _udp;
    private readonly ulong _session;
    private readonly object _lock = new();
    private readonly Dictionary<uint, Pkt> _inflight = new();
    private readonly SemaphoreSlim _slots = new(RudpConst.Window, RudpConst.Window);
    private readonly CancellationTokenSource _cts = new();
    private uint _next, _base;
    private double _srtt = 100;   // ms
    private Exception _fault;
    private long _retx, _packets, _wireBytes;

    public long Retransmissions => Interlocked.Read(ref _retx);
    public long PacketsSent => Interlocked.Read(ref _packets);
    /// <summary>Tổng byte đã đưa lên đường truyền (gồm header UDP/IP ước tính và gói gửi lại).</summary>
    public long WireBytes => Interlocked.Read(ref _wireBytes);

    public RudpSender(IPEndPoint remote, ulong session)
    {
        _session = session;
        _udp = new UdpClient(remote.AddressFamily);
        _udp.Client.ReceiveBufferSize = 1 << 20;
        _udp.Connect(remote);
        _ = Task.Run(AckLoopAsync);
        _ = Task.Run(RetransmitLoopAsync);
    }

    private void ThrowIfFaulted()
    {
        var f = _fault;
        if (f != null) throw new IOException(f.Message, f);
    }

    private void Transmit(byte[] wire)
    {
        try
        {
            _udp.Send(wire, wire.Length);
            Interlocked.Increment(ref _packets);
            Interlocked.Add(ref _wireBytes, wire.Length + 28);   // + IP(20) + UDP(8)
        }
        catch (SocketException) { }
        catch (ObjectDisposedException) { }
    }

    private async Task SendPacketAsync(byte type, ReadOnlyMemory<byte> payload, CancellationToken ct)
    {
        ThrowIfFaulted();
        try
        {
            using var link = CancellationTokenSource.CreateLinkedTokenSource(ct, _cts.Token);
            await _slots.WaitAsync(link.Token);
        }
        catch (OperationCanceledException) when (_fault != null)
        {
            ThrowIfFaulted();
        }

        Pkt p;
        lock (_lock)
        {
            var wire = new byte[RudpConst.HeaderLen + payload.Length];
            wire[0] = type;
            BinaryPrimitives.WriteUInt64BigEndian(wire.AsSpan(1), _session);
            BinaryPrimitives.WriteUInt32BigEndian(wire.AsSpan(9), _next);
            payload.Span.CopyTo(wire.AsSpan(RudpConst.HeaderLen));
            p = new Pkt { Seq = _next, Wire = wire, SentAt = Environment.TickCount64, Tries = 1 };
            _inflight[_next] = p;
            _next++;
        }
        Transmit(p.Wire);
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct = default)
    {
        int offset = 0;
        while (offset < buffer.Length)
        {
            int n = Math.Min(RudpConst.Payload, buffer.Length - offset);
            await SendPacketAsync(RudpConst.Data, buffer.Slice(offset, n), ct);
            offset += n;
        }
    }

    /// <summary>Gửi gói FIN và chờ mọi gói được xác nhận.</summary>
    public async Task CompleteAsync(CancellationToken ct)
    {
        await SendPacketAsync(RudpConst.Fin, ReadOnlyMemory<byte>.Empty, ct);
        long deadline = Environment.TickCount64 + 30_000;
        while (true)
        {
            ThrowIfFaulted();
            lock (_lock)
            {
                if (_base == _next) return;
            }
            if (Environment.TickCount64 > deadline)
                throw new TimeoutException("Hết thời gian chờ xác nhận cuối cùng của UDP.");
            await Task.Delay(10, ct);
        }
    }

    private async Task AckLoopAsync()
    {
        var ct = _cts.Token;
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var r = await _udp.ReceiveAsync(ct);
                HandleAck(r.Buffer);
            }
            catch (OperationCanceledException) { return; }
            catch (ObjectDisposedException) { return; }
            catch (SocketException)
            {
                try { await Task.Delay(10, ct); }
                catch (OperationCanceledException) { return; }
            }
        }
    }

    private void HandleAck(byte[] b)
    {
        if (b.Length < 21 || b[0] != RudpConst.Ack) return;
        if (BinaryPrimitives.ReadUInt64BigEndian(b.AsSpan(1)) != _session) return;
        uint cum = BinaryPrimitives.ReadUInt32BigEndian(b.AsSpan(9));
        ulong bitmap = BinaryPrimitives.ReadUInt64BigEndian(b.AsSpan(13));
        int release = 0;

        lock (_lock)
        {
            if (cum > _next) return;                 // ACK không hợp lệ
            long now = Environment.TickCount64;
            for (uint s = _base; s < cum; s++)
            {
                if (_inflight.Remove(s, out var p) && p.Tries == 1)
                    _srtt = 0.875 * _srtt + 0.125 * Math.Max(1, now - p.SentAt);
            }
            if (cum > _base)
            {
                release = (int)(cum - _base);
                _base = cum;
            }
            for (int i = 0; i < 64; i++)
            {
                if ((bitmap & (1UL << i)) != 0 && _inflight.TryGetValue(cum + 1 + (uint)i, out var q))
                    q.Acked = true;
            }
        }
        if (release > 0) _slots.Release(release);
    }

    private async Task RetransmitLoopAsync()
    {
        var ct = _cts.Token;
        try
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(20));
            while (await timer.WaitForNextTickAsync(ct))
            {
                long now = Environment.TickCount64;
                long rto = (long)Math.Clamp(_srtt * 2.5, 60, 1000);
                List<Pkt> due = null;
                lock (_lock)
                {
                    foreach (var p in _inflight.Values)
                    {
                        if (p.Acked) continue;
                        if (now - p.SentAt < rto * (1L << Math.Min(p.Tries - 1, 2))) continue;
                        if (p.Tries >= 25)
                        {
                            _fault = new IOException("Mất kết nối UDP: không nhận được xác nhận từ server.");
                            break;
                        }
                        p.Tries++;
                        p.SentAt = now;
                        (due ??= new List<Pkt>()).Add(p);
                    }
                }
                if (_fault != null)
                {
                    _cts.Cancel();
                    return;
                }
                if (due != null)
                {
                    foreach (var p in due)
                    {
                        Transmit(p.Wire);
                        Interlocked.Increment(ref _retx);
                    }
                }
            }
        }
        catch (OperationCanceledException) { }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try { _cts.Cancel(); } catch { }
            _udp.Dispose();
        }
        base.Dispose(disposing);
    }

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    public override void Flush() { }
    public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count)
        => WriteAsync(buffer.AsMemory(offset, count)).AsTask().GetAwaiter().GetResult();
}

/// <summary>Phía nhận: là một Stream chỉ đọc, trả byte theo đúng thứ tự gửi.</summary>
public sealed class RudpReceiver : Stream
{
    private readonly ulong _session;
    private readonly Action<byte[], IPEndPoint> _sendTo;
    private readonly Channel<byte[]> _queue = Channel.CreateUnbounded<byte[]>();
    private readonly Dictionary<uint, (byte[] Data, bool Fin)> _buf = new();
    private uint _expected;
    private byte[] _cur;
    private int _pos;

    internal RudpReceiver(ulong session, Action<byte[], IPEndPoint> sendTo)
    {
        _session = session;
        _sendTo = sendTo;
    }

    internal void OnPacket(byte type, uint seq, ReadOnlySpan<byte> payload, IPEndPoint from)
    {
        var ack = new byte[21];
        lock (_buf)
        {
            if (seq >= _expected && seq - _expected < RudpConst.Window * 2 && !_buf.ContainsKey(seq))
                _buf[seq] = (payload.ToArray(), type == RudpConst.Fin);

            while (_buf.Remove(_expected, out var item))
            {
                if (item.Data.Length > 0) _queue.Writer.TryWrite(item.Data);
                _expected++;
                if (item.Fin) _queue.Writer.TryComplete();
            }

            ulong bitmap = 0;
            for (int i = 0; i < 64; i++)
                if (_buf.ContainsKey(_expected + 1 + (uint)i)) bitmap |= 1UL << i;

            ack[0] = RudpConst.Ack;
            BinaryPrimitives.WriteUInt64BigEndian(ack.AsSpan(1), _session);
            BinaryPrimitives.WriteUInt32BigEndian(ack.AsSpan(9), _expected);
            BinaryPrimitives.WriteUInt64BigEndian(ack.AsSpan(13), bitmap);
        }
        _sendTo(ack, from);
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken ct = default)
    {
        if (buffer.Length == 0) return 0;
        while (_cur == null || _pos >= _cur.Length)
        {
            if (!await _queue.Reader.WaitToReadAsync(ct)) return 0;
            if (_queue.Reader.TryRead(out var next))
            {
                _cur = next;
                _pos = 0;
            }
        }
        int n = Math.Min(buffer.Length, _cur.Length - _pos);
        _cur.AsMemory(_pos, n).CopyTo(buffer);
        _pos += n;
        return n;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _queue.Writer.TryComplete();
        base.Dispose(disposing);
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    public override void Flush() { }
    public override int Read(byte[] buffer, int offset, int count)
        => ReadAsync(buffer.AsMemory(offset, count)).AsTask().GetAwaiter().GetResult();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}

/// <summary>Một socket UDP chung của server: phân phối gói theo SessionId tới đúng RudpReceiver.</summary>
public sealed class RudpHub : IDisposable
{
    private readonly UdpClient _udp;
    private readonly ConcurrentDictionary<ulong, RudpReceiver> _recv = new();
    private readonly CancellationTokenSource _cts = new();

    public RudpHub(int port)
    {
        _udp = new UdpClient(new IPEndPoint(IPAddress.Any, port));
        _udp.Client.ReceiveBufferSize = 4 * 1024 * 1024;
        try
        {
            // tắt lỗi "connection reset" do ICMP port-unreachable trên Windows
            _udp.Client.IOControl((IOControlCode)(-1744830452), new byte[] { 0 }, null);
        }
        catch { }
        _ = Task.Run(LoopAsync);
    }

    public RudpReceiver Register(ulong session)
    {
        var r = new RudpReceiver(session, SendTo);
        _recv[session] = r;
        return r;
    }

    public void Unregister(ulong session)
    {
        if (_recv.TryRemove(session, out var r)) r.Dispose();
    }

    private void SendTo(byte[] wire, IPEndPoint to)
    {
        try { _udp.Client.SendTo(wire, to); }
        catch (SocketException) { }
        catch (ObjectDisposedException) { }
    }

    private async Task LoopAsync()
    {
        var ct = _cts.Token;
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var r = await _udp.ReceiveAsync(ct);
                var b = r.Buffer;
                if (b.Length < RudpConst.HeaderLen) continue;
                byte type = b[0];
                if (type != RudpConst.Data && type != RudpConst.Fin) continue;
                ulong session = BinaryPrimitives.ReadUInt64BigEndian(b.AsSpan(1));
                if (!_recv.TryGetValue(session, out var rcv)) continue;
                uint seq = BinaryPrimitives.ReadUInt32BigEndian(b.AsSpan(9));
                rcv.OnPacket(type, seq, b.AsSpan(RudpConst.HeaderLen), r.RemoteEndPoint);
            }
            catch (OperationCanceledException) { return; }
            catch (ObjectDisposedException) { return; }
            catch (SocketException) { }
        }
    }

    public void Dispose()
    {
        try { _cts.Cancel(); } catch { }
        _udp.Dispose();
        foreach (var r in _recv.Values) r.Dispose();
        _recv.Clear();
    }
}
