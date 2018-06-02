using DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            GetList();
        }

        private async void CopySelect(object sender, RoutedEventArgs e)
        {
            if (ClipboardListView.SelectedItems.Count > 0)
            {
                var list = ClipboardListView.SelectedItems.Cast<ClipboardItem>();
                var str = list.Aggregate("", (sum, next) => $"{sum}\r\n{next.Text}");
                await AppServiceReciver.Invoke(s => s.SetClipboard(new ClipboardItem(str)));
            }
        }

        private async void ClipboardItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ClipboardListView.SelectedItem = null;
            var TextBlock = (TextBlock)sender;
            var item = TextBlock.Tag as ClipboardItem;
            var x = await AppServiceReciver.Invoke(s => s.SetClipboard(item));
        }

        private bool isTopmost = false;

        private async void TopmostToggle(object sender, RoutedEventArgs e)
        {
            TopmostButton.IsEnabled = false;
            isTopmost = !isTopmost;
            var x = (bool)await AppServiceReciver.Invoke(s => s.SetTopmost(isTopmost));
            TopmostButton.Icon = new SymbolIcon(x ? Symbol.Pin : Symbol.UnPin);
            TopmostButton.IsEnabled = true;
        }

        private async void ClearLists(object sender, RoutedEventArgs e)
        {
            var x = (bool)await AppServiceReciver.Invoke(s => s.ClearClipboardList());
            if (x)
            {
                Client.Instance.ClipboardContents.Clear();
            }
        }

        private async void GetList(object sender = null, RoutedEventArgs e = null)
        {
            await Client.Instance.RequestClipboardContentsAsync();
            ClipboardListView.ItemsSource = Client.Instance.ClipboardContents?.List;
        }
    }
}
