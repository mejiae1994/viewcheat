using System.Configuration;
using System.Windows;

namespace Wpftest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            MainWindow mainWindow = new MainWindow(config);
            mainWindow.Show();

        }
    }
}
