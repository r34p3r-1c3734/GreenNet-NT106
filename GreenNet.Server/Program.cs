namespace GreenNet.Server;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (s, e) =>
            MessageBox.Show(e.Exception.Message, "Lỗi không mong muốn", MessageBoxButtons.OK, MessageBoxIcon.Error);
        Application.Run(new frmServer());
    }
}
