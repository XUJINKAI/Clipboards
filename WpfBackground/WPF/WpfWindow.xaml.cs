using DataModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
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
            Log.TextListener -= Log_TextListener;
            Log.AutoFlush = false;
        }

        public static void ShowWindow()
        {
            if(Instance == null)
            {
                Instance = new WpfWindow();
                Instance.DataContext = Instance;
                Log.TextListener += Log_TextListener;
                Log.AutoFlush = true;
            }
            Instance.Show();
            Instance.Activate();
        }

        private static void Log_TextListener(string obj)
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

        private async void GetUiState(object sender, RoutedEventArgs e)
        {
            var result = await Service.ClientProxy.UiShowed();
            Debug.WriteLine(result);
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            true.GetType().DefaultValue();
            1.GetType().DefaultValue();
            "abc".GetType().DefaultValue();
            TypeHelper.DefaultValue(null);
            Task.FromResult<object>(null).GetType().DefaultValue();
            Task.FromResult<int>(123).GetType().DefaultValue();
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            Service.AppServiceInvoker.TryConnect();
        }
    }
}
