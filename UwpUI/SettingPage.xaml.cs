using DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public sealed partial class SettingPage : Page, INotifyPropertyChanged
    {
        public ClipboardSetting Setting { get; set; } = null;

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
            await Controller.ServerProxy.ShowWpfWindow();
        }

        public void GetSetting()
        {
            Setting = Controller.ClipboardWrapper.Setting;
            OnPropertyChanged("Setting");
        }

        private async void SetSetting(object sender, RoutedEventArgs e)
        {
            if (Setting != null)
            {

            }
        }

        private async void SaveData(object sender, RoutedEventArgs e)
        {
            await Controller.ServerProxy.WriteDataFile();
        }
    }
}
