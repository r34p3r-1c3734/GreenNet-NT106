using GreenNet.Core;

namespace GreenNet.Client;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (s, e) =>
            MessageBox.Show(e.Exception.Message, "Lỗi không mong muốn", MessageBoxButtons.OK, MessageBoxIcon.Error);

        TrustStore.FileName = "known_servers.client.json";
        SecureChannel.ConfirmNewServer = ClientSession.ConfirmNewServer;
        try
        {
            EnergyModel.LoadCalibration();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Không đọc được energy_calibration.json, dùng giá trị mặc định.\n" + ex.Message,
                "Hiệu chỉnh năng lượng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        Application.Run(new frmLogin());
    }
}
