using DataModel;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UWPUI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ClipboardContentPage : Page
    {
        public ClipboardContentPage()
        {
            this.InitializeComponent();
            App.Recived += App_Recived;
            GetLists();
        }

        private void App_Recived(AnswerData obj)
        {
            if(obj.Request == Request.AnsClipboardList)
            {
                ClipboardListView.ItemsSource = ((ExClipboardList)obj.Data).Data;
            }
        }

        public void GetLists()
        {
            App.Send(new RequestData(Request.AskClipboardList));
        }
    }
}
