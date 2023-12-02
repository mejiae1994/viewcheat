using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace Wpftest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon notyIcon;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private HwndSource _source;
        private const int HOTKEY_ID = 9000;
        private const int WM_HOTKEY = 0x0312;
        private const string ImageFolderName = "Images";

        public IEnumerable<string> ImagePaths { get; set; }

        enum keymodifier
        {
            none = 0,
            alt = 1,
            control = 2,
            shift = 4,
            winkey = 8
        }

        public MainWindow()
        {
            InitializeComponent();
            AddNotifyIcon();
            LoadImgPaths();
        }

        private void AddNotifyIcon()
        {
            notyIcon = new NotifyIcon();

            // Get the image resource
            notyIcon.Icon = Resource1.Icon;
            notyIcon.Click += new EventHandler(TrayIconClick);
            notyIcon.Visible = true;

            void TrayIconClick(object? sender, EventArgs e)
            {
                BringWindowToFront();
            }
        }

        private void LoadImgPaths()
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory + ImageFolderName;

            if (Directory.Exists(currentDirectory))
            {
                var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                try
                {
                    ImagePaths = Directory.GetFiles(currentDirectory).Where(file => imageExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)));
                    ImgList.ItemsSource = ImagePaths;
                }
                catch (IOException ex)
                {
                    // Handle general IO exception
                    Console.WriteLine($"IO exception: {ex.Message}");
                }
            }
        }

        private void ActivateWindowToFront()
        {
            if (WindowState == WindowState.Minimized)
            {
                BringWindowToFront();
            }
            else if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void BringWindowToFront()
        {
            WindowState = WindowState.Normal;
            Activate();
            Focus();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                ShowInTaskbar = false;
            }
            else
            {
                ShowInTaskbar = true;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey(helper.Handle, HOTKEY_ID, (int)keymodifier.control, (int)Keys.F7);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ImgList.Focus();
            ImgList.SelectedIndex = 0;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
        }

        private void ImgList_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ImgList.SelectedItem != null)
                {
                    // Get the selected item
                    string selectedItemPath = ImgList.SelectedItem as string;

                    // Pass the selected item to your desired method or event handler
                    if (!String.IsNullOrEmpty(selectedItemPath))
                    {
                        OpenFileByPath(selectedItemPath);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Incorrect Image Path");
                    }
                }
            }
        }

        private void OpenFileByPath(string path)
        {
            string argument = "/open, \"" + path + "\"";

            try
            {
                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening file: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
            base.OnClosed(e);
        }

        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            if (msg == WM_HOTKEY)
            {
                switch (wParam.ToInt32())
                {
                    case HOTKEY_ID:
                        ActivateWindowToFront();
                        handled = true;
                        break;
                }
            }
            return IntPtr.Zero;
        }
    }
}
