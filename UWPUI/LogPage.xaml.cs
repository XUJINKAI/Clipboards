using Windows.UI.Xaml.Controls;
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
            Log.AutoStringListener += Log_AutoStringListener;
            LogTextBlock.Text = Log.AllCachesText;
        }

        private void Log_AutoStringListener(string obj)
        {
            LogTextBlock.Text += obj;
        }
    }
}
