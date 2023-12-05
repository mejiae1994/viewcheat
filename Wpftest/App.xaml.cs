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
            string currentDirectory = config.AppSettings.Settings["directoryPath"].Value;
            MainViewModel viewModel = new MainViewModel(currentDirectory);
            MainWindow mainWindow = new MainWindow(config);
            mainWindow.DataContext = viewModel;
            mainWindow.Show();
        }
    }
}
