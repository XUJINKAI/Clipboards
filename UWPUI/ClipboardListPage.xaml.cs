using DataModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UWPUI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ClipboardListPage : Page
    {
        public ClipboardListPage()
        {
            this.InitializeComponent();
            Client.Current.TopmostChanged += Current_TopmostChanged;
            ClipboardListView.ItemsSource = Client.ClipboardContents;
        }

        private async void CopySelect(object sender, RoutedEventArgs e)
        {
            if (ClipboardListView.SelectedItems.Count == 1)
            {
                var item = ClipboardListView.SelectedItem as ClipboardItem;
                await Client.ServerProxy.SetClipboard(item);
            }
            else if (ClipboardListView.SelectedItems.Count > 1)
            {
                var list = ClipboardListView.SelectedItems.Cast<ClipboardItem>();
                var str = list.Aggregate("", (sum, next) => $"{sum}\r\n{next.Text}");
                await Client.ServerProxy.SetClipboard(new ClipboardItem(str));
            }
        }

        private async void ClipboardItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ClipboardListView.SelectedItem = null;
            var TextBlock = (TextBlock)sender;
            var item = TextBlock.Tag as ClipboardItem;
            await Client.ServerProxy.SetClipboard(item);
        }

        private async void ClearLists(object sender, RoutedEventArgs e)
        {
            var x = await Client.ServerProxy.ClearClipboardList();
            if (x)
            {
                Client.ClipboardContents.Clear();
            }
        }

        private async void GetList(object sender = null, RoutedEventArgs e = null)
        {
            await Client.Current.RequestClipboardContentsAsync();
        }

        private async void TopmostButton_Click(object sender, RoutedEventArgs e)
        {
            TopmostButton.IsEnabled = false;
            await Client.Current.SetTopmostAsync(!Client.Current.IsTopmost);
            TopmostButton.IsEnabled = true;
        }

        private void Current_TopmostChanged(bool obj)
        {
            TopmostButton.Icon = new SymbolIcon(obj ? Symbol.Pin : Symbol.UnPin);
        }

    }
}
