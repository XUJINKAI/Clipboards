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
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XJK;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UwpUI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LogPage : Page
    {
        public LogPage()
        {
            this.InitializeComponent();
            Log.TextListener += Log_TextListener;
            Log_TextListener(Log.AllCachesText);
        }

        private void Log_TextListener(string obj)
        {
            LogTextSpan.Inlines.Add(new Run() { Text = obj });
        }

        private void ClearLogButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            LogTextSpan.Inlines.Clear();
        }
    }
}
