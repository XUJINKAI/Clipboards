using DataModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using XJK;

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
                Instance.DataContext = Instance;
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
            ClipboardsListView.ItemsSource = Clipboards.Contents;
        }

        private void Copy_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = ClipboardsListView.SelectedItem as ClipboardItem;
            Service.Current.SetClipboard(item);
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Service.Current.Shutdown();
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            Service.Current.ClearClipboardList();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            Service.Current.WriteDataFile();
        }
    }
}
