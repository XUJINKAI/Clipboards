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
            ClipboardListView.ItemsSource = AppServer.ClipboardContents;
        }

        private void CopySelect(object sender, RoutedEventArgs e)
        {
            if (ClipboardListView.SelectedItems.Count > 0)
            {
                var list = ClipboardListView.SelectedItems.Cast<ClipboardItem>();
                var str = list.Aggregate("", (sum, next) => $"{sum}\r\n{next.Text}");
                AppServer.SetClipboard(str);
            }
        }

        private void ClipboardItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var TextBlock = (TextBlock)sender;
            var text = TextBlock.Tag as string;
            AppServer.SetClipboard(text);
            ClipboardListView.SelectedItem = null;
        }

        private bool isTopmost = false;

        private void TopmostToggle(object sender, RoutedEventArgs e)
        {
            isTopmost = !isTopmost;
            AppServer.RequestSetTopmost(isTopmost);
            TopmostButton.Icon = new SymbolIcon(isTopmost ? Symbol.Pin : Symbol.UnPin);
        }

        private void ClearLists(object sender, RoutedEventArgs e)
        {
            AppServer.RequestClearClipboardList();
        }

        private void GetList(object sender, RoutedEventArgs e)
        {
            AppServer.RequestGetClipboardList();
        }
    }
}
