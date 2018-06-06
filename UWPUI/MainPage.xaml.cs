using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace UWPUI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            rootFrame.Navigate(typeof(ClipboardListPage));
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                rootFrame.Navigate(typeof(SettingPage));
            }
            else
            {
                var item = (NavigationViewItem)args.SelectedItem;
                var tag = item.Tag as string;
                switch (tag)
                {
                    case "History":
                        rootFrame.Navigate(typeof(ClipboardListPage));
                        break;
                    case "Log":
                        rootFrame.Navigate(typeof(LogPage));
                        break;
                    case "Exit":
                        Client.Current.ExitBackground();
                        break;
                }
            }
        }
    }
}
