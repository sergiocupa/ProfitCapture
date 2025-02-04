using Microsoft.VisualBasic.Logging;
using ProfitCapture.UI.Template;

namespace ProfitCapture
{
    internal static class Program
    {

        private static void FormPrincipal_UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            try
            {
                MessageBox.Show(t.Exception.Message);
            }
            catch (Exception ex)
            {
            }
            Environment.Exit(0);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var ex = (Exception)e.ExceptionObject;
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
            }
            Environment.Exit(0);
        }



        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();


            Application.ThreadException += new ThreadExceptionEventHandler(FormPrincipal_UIThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);


            Application.Run(new Form1());
        }
    }
}