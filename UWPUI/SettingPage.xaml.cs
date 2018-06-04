using DataModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UWPUI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            this.InitializeComponent();
            PackageFamilyNameTextBox.Text = Windows.ApplicationModel.Package.Current.Id.FamilyName;
        }

        private void ExitBackground(object sender, RoutedEventArgs e)
        {
            Client.Current.ExitBackground();
        }

        private async void ShowWpfWindow(object sender, RoutedEventArgs e)
        {
            await Client.ServerProxy.ShowUwpWindow();
        }

    }
}
