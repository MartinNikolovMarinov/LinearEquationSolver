using System.Globalization;
using System.Threading;
using System.Windows;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            MainWindow wnd = new MainWindow();
            wnd.Show();
        }
    }
}
