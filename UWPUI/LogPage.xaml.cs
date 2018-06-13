using System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using XJK;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UWPUI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LogPage : Page
    {
        public LogPage()
        {
            this.InitializeComponent();
            Log.TextListener += Log_AutoStringListener;
            LogTextSpan.Inlines.Add(new Run() { Text = Log.AllCachesText });
        }

        private void Log_AutoStringListener(string obj)
        {
            LogTextSpan.Inlines.Add(new Run() { Text = obj });
        }
    }
}
