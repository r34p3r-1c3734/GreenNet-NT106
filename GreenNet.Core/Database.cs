using Microsoft.Data.SqlClient;

namespace GreenNet.Core;

public class Database
{
    private readonly string _cs;
    public Database(string connectionString) => _cs = connectionString;

    private async Task<SqlConnection> OpenAsync()
    {
        var c = new SqlConnection(_cs);
        await c.OpenAsync();
        return c;
    }

    public async Task PingAsync()
    {
        using var c = await OpenAsync();
        using var cmd = new SqlCommand("SELECT 1", c);
        await cmd.ExecuteScalarAsync();
    }

    public async Task<bool> CreateUserAsync(string user, string hash)
    {
        try
        {
            using var c = await OpenAsync();
            using var cmd = new SqlCommand(
                "IF NOT EXISTS (SELECT 1 FROM Users WHERE Username=@u) " +
                "BEGIN INSERT INTO Users(Username, PasswordHash) VALUES(@u, @h); SELECT 1; END ELSE SELECT 0;", c);
            cmd.Parameters.AddWithValue("@u", user);
            cmd.Parameters.AddWithValue("@h", hash);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync()) == 1;
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            return false;   // trùng khóa do đăng ký đồng thời
        }
    }

    public async Task<string> GetPasswordHashAsync(string user)
    {
        using var c = await OpenAsync();
        using var cmd = new SqlCommand("SELECT PasswordHash FROM Users WHERE Username=@u", c);
        cmd.Parameters.AddWithValue("@u", user);
        return (await cmd.ExecuteScalarAsync()) as string;
    }

    public async Task<string> CreateTokenAsync(string user)
    {
        var token = Guid.NewGuid().ToString();
        using var c = await OpenAsync();
        using var cmd = new SqlCommand(
            "DELETE FROM Tokens WHERE ExpiresAt < SYSUTCDATETIME(); " +
            "INSERT INTO Tokens(Token, Username, ExpiresAt) VALUES(@t, @u, DATEADD(HOUR, 8, SYSUTCDATETIME()));", c);
        cmd.Parameters.AddWithValue("@t", token);
        cmd.Parameters.AddWithValue("@u", user);
        await cmd.ExecuteNonQueryAsync();
        return token;
    }

    public async Task<string> GetUserByTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token) || token.Length != 36) return null;
        using var c = await OpenAsync();
        using var cmd = new SqlCommand(
            "SELECT Username FROM Tokens WHERE Token=@t AND ExpiresAt > SYSUTCDATETIME()", c);
        cmd.Parameters.AddWithValue("@t", token);
        return (await cmd.ExecuteScalarAsync()) as string;
    }

    public async Task InsertSessionAsync(string user, SessionRow r)
    {
        using var c = await OpenAsync();
        using var cmd = new SqlCommand(
            "INSERT INTO Sessions(Username, ServerName, FileName, FileSizeBytes, BytesSent, Algo, Protocol, Encrypted, " +
            "DurationMs, RttMs, CpuMs, EnergyJ, GreenScore, Sha256Ok) " +
            "VALUES(@u,@sv,@fn,@fs,@bs,@al,@pr,@en,@du,@rt,@cp,@ej,@gs,@ok)", c);
        cmd.Parameters.AddWithValue("@u", user);
        cmd.Parameters.AddWithValue("@sv", r.ServerName ?? "");
        cmd.Parameters.AddWithValue("@fn", r.FileName ?? "");
        cmd.Parameters.AddWithValue("@fs", r.FileSizeBytes);
        cmd.Parameters.AddWithValue("@bs", r.BytesSent);
        cmd.Parameters.AddWithValue("@al", r.Algo ?? "None");
        cmd.Parameters.AddWithValue("@pr", r.Protocol ?? "TCP");
        cmd.Parameters.AddWithValue("@en", r.Encrypted);
        cmd.Parameters.AddWithValue("@du", r.DurationMs);
        cmd.Parameters.AddWithValue("@rt", r.RttMs);
        cmd.Parameters.AddWithValue("@cp", r.CpuMs);
        cmd.Parameters.AddWithValue("@ej", r.EnergyJ);
        cmd.Parameters.AddWithValue("@gs", r.GreenScore);
        cmd.Parameters.AddWithValue("@ok", r.Sha256Ok);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<SessionRow>> GetSessionsAsync(string user)
    {
        var list = new List<SessionRow>();
        using var c = await OpenAsync();
        using var cmd = new SqlCommand(
            "SELECT TOP 1000 Id, CreatedAt, ServerName, FileName, FileSizeBytes, BytesSent, Algo, Encrypted, " +
            "DurationMs, RttMs, CpuMs, EnergyJ, GreenScore, Sha256Ok, Protocol FROM Sessions WHERE Username=@u ORDER BY Id DESC", c);
        cmd.Parameters.AddWithValue("@u", user);
        using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
        {
            list.Add(new SessionRow
            {
                Id = rd.GetInt32(0),
                CreatedAt = DateTime.SpecifyKind(rd.GetDateTime(1), DateTimeKind.Utc),
                ServerName = rd.GetString(2),
                FileName = rd.GetString(3),
                FileSizeBytes = rd.GetInt64(4),
                BytesSent = rd.GetInt64(5),
                Algo = rd.GetString(6),
                Encrypted = rd.GetBoolean(7),
                DurationMs = rd.GetDouble(8),
                RttMs = rd.GetDouble(9),
                CpuMs = rd.GetDouble(10),
                EnergyJ = rd.GetDouble(11),
                GreenScore = rd.GetDouble(12),
                Sha256Ok = rd.GetBoolean(13),
                Protocol = rd.GetString(14)
            });
        }
        return list;
    }
}
