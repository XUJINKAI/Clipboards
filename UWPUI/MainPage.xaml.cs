using CommonLibrary;
using DataModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace UWPUI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public static MainPage Instance;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        public static ObservableCollection<string> ClipboardText = new ObservableCollection<string>();

        public MainPage()
        {
            this.InitializeComponent();
            Log.AutoStringListener += Log_AutoStringListener;
            Log.AutoFlush = true;
            Instance = this;
            AppServer.RequestLists();
            PackageFamilyNameTextBox.Text = Windows.ApplicationModel.Package.Current.Id.FamilyName;
            ClipboardListView.ItemsSource = ClipboardText;
        }
        
        private void Log_AutoStringListener(string obj)
        {
            LogTextBox.Text += obj;
        }

        public void AddClipboardList(string text)
        {
            if (!ClipboardText.Contains(text))
            {
                ClipboardText.Insert(0, text);
            }
        }

        public void SetClipboardList(List<string> data)
        {
            data.Reverse();
            ClipboardText.Clear();
            foreach(var x in data)
            {
                ClipboardText.Add(x);
            }
        }

        private void ExitBackground(object sender, RoutedEventArgs e)
        {
            App.Send(new ConnectionData(Direction.AskServer, Command.ShutDown));
            App.Current.Exit();
        }

        private void ShowWpfWindow(object sender, RoutedEventArgs e)
        {
            App.Send(new ConnectionData(Direction.AskServer, Command.ShowWindow));
        }

        private void TextBlock_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            var TextBlock = (TextBlock)sender;
            var data = new DataPackage();
            data.SetText(TextBlock.Text);
            Clipboard.SetContent(data);
        }
        
    }
}
