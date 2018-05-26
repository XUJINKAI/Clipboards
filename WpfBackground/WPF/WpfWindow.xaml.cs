using System.ComponentModel;
using System.Windows;

namespace WpfBackground
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WpfWindow : Window
    {
        public static WpfWindow Instance;

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
            Instance = this;
            Clipboards.Changed += Clipboards_Changed;
        }

        private void Clipboards_Changed()
        {
            ClipboardsListView.ItemsSource = null;
            ClipboardsListView.ItemsSource = Clipboards.TextList;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Shutdown();
        }

        private void Shutdown()
        {
            App.Current.Shutdown();
        }
    }
}
