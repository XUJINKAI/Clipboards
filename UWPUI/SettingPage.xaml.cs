using DataModel;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XJK;
using XJK.Serializers;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UWPUI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page, INotifyPropertyChanged
    {
        public Setting Setting { get; set; } = null;

#if DEBUG
        public Visibility DebugVisibility => Visibility.Visible;
#else
        public Visibility DebugVisibility => Visibility.Collapsed;
#endif

        public SettingPage()
        {
            this.InitializeComponent();
            DataContext = this;
            GetSetting();
            PackageFamilyNameTextBox.Text = Windows.ApplicationModel.Package.Current.Id.FamilyName;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        
        private async void ShowWpfWindow(object sender, RoutedEventArgs e)
        {
            await Client.ServerProxy.ShowUwpWindow();
        }

        public async void GetSetting()
        {
            Setting = await Client.ServerProxy.GetSetting();
            OnPropertyChanged("Setting");
        }

        private async void SaveSetting(object sender, RoutedEventArgs e)
        {
            if (Setting != null)
            {
                var result = await Client.ServerProxy.SetSetting(Setting);
            }
        }

        private async void SaveData(object sender, RoutedEventArgs e)
        {
            await Client.ServerProxy.WriteDataFile();
        }
    }
}
