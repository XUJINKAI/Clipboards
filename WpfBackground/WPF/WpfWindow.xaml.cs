using CommonLibrary;
using System.ComponentModel;
using System.Windows;

namespace WpfBackground
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WpfWindow : Window
    {
        public static WpfWindow Instance = null;

        protected override void OnClosing(CancelEventArgs e)
        {
            Instance = null;
            Log.AutoStringListener -= Log_AutoStringListener;
            Log.AutoFlush = false;
        }

        public static void ShowWindow()
        {
            if(Instance == null)
            {
                Instance = new WpfWindow();
                Log.AutoStringListener += Log_AutoStringListener;
                Log.AutoFlush = true;
            }
            Instance.Show();
            Instance.Activate();
        }

        private static void Log_AutoStringListener(string obj)
        {
            Instance.Dispatcher.Invoke(() =>
            {
                Instance.AddLog(obj);
            });
        }

        public void AddLog(string obj)
        {
            LogTextBox.AppendText(obj);
        }

        public WpfWindow()
        {
            InitializeComponent();
            ClipboardsListView.ItemsSource = Clipboards.TextList;
            Clipboards.Changed += Clipboards_Changed;
        }
        
        private void Clipboards_Changed(string text)
        {
            ClipboardsListView.ItemsSource = null;
            ClipboardsListView.ItemsSource = Clipboards.TextList;
        }
        
        private void Shutdown()
        {
            App.ShutDown();
        }

        private async void OpenConnect(object sender, RoutedEventArgs e)
        {
            var result = await AppServiceConnect.ConnectionAsync(App.AppServerName);
            MessageBox.Show(result);
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Shutdown();
        }
    }
}
