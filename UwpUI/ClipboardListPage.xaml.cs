using DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace UwpUI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ClipboardListPage : Page
    {
        public ClipboardListPage()
        {
            this.InitializeComponent();
            UpdateTopmostSymbolIcon();
            ClipboardListView.ItemsSource = Controller.ClipboardWrapper.Contents;
        }

        private void UpdateTopmostSymbolIcon(bool? topmost = null)
        {
            if (topmost == null)
            {
                topmost = false;
            }
            TopmostButton.Icon = new SymbolIcon(topmost.Value ? Symbol.Pin : Symbol.UnPin);
        }

        private async void TopmostButton_Click(object sender, RoutedEventArgs e)
        {
            TopmostButton.IsEnabled = false;
            //await Controller.ServerProxy.(!Client.Current.IsTopmost);
            //UpdateTopmostSymbolIcon
            TopmostButton.IsEnabled = true;
        }


        private async void CopySelect(object sender, RoutedEventArgs e)
        {
            if (ClipboardListView.SelectedItems.Count == 1)
            {
                var item = ClipboardListView.SelectedItem as ClipboardItem;
                await Controller.ServerProxy.SetClipboard(item);
            }
            else if (ClipboardListView.SelectedItems.Count > 1)
            {
                var list = ClipboardListView.SelectedItems.Cast<ClipboardItem>().ToList();
                await Controller.ServerProxy.SetClipboard(list);
            }
        }
        
        private async void ClipboardItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ClipboardListView.SelectedItem = null;
            var Grid = (Grid)sender;
            var item = Grid.Tag as ClipboardItem;
            await Controller.ServerProxy.SetClipboard(item);
        }
        
        private async void ClearLists(object sender, RoutedEventArgs e)
        {
            var x = await Controller.ServerProxy.ClearClipboardList();
            if (x)
            {
                Controller.ClipboardWrapper.Contents.Clear();
            }
        }
        
        private async void RequestList(object sender = null, RoutedEventArgs e = null)
        {
            await Controller.Instance.RequestClipboardContentsAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            await Controller.ServerProxy.WriteDataFile();
        }

        private async void TEST_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
