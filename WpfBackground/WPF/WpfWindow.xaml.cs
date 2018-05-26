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
        }

        public static void ShowWindow()
        {
            if(Instance == null)
            {
                Instance = new WpfWindow();
            }
            Instance.Show();
            Instance.Activate();
        }

        public WpfWindow()
        {
            InitializeComponent();
            ClipboardsListView.ItemsSource = Clipboards.TextList;
            Clipboards.Changed += Clipboards_Changed;
        }
        
        private void Clipboards_Changed()
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
            var result = await AppServiceConnect.OpenConnection(App.AppServerName);
            MessageBox.Show(result);
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Shutdown();
        }
    }
}
