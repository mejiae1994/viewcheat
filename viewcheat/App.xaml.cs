using System;
using System.Configuration;
using System.IO;
using System.Windows;

namespace viewcheat
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
            string configDirectory = config.AppSettings.Settings["directoryPath"].Value;

            if (String.IsNullOrEmpty(configDirectory))
            {
                config.AppSettings.Settings["directoryPath"].Value = Directory.GetCurrentDirectory();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
            }

            MainViewModel viewModel = new MainViewModel(config.AppSettings.Settings["directoryPath"].Value);
            MainWindow mainWindow = new MainWindow(config);
            mainWindow.DataContext = viewModel;
            mainWindow.Show();
        }
    }
}
