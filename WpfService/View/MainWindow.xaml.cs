using DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfService.Model;
using XJK;
using XJK.Serializers;

namespace WpfService
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }


        public MainWindow()
        {
            InitializeComponent();
            ClipboardContentListView.SelectionChanged += ClipboardContentListView_SelectionChanged;
            LogText = Log.AllCachesText;
            IsLogging = App.DEBUG;
            LoadFile();
        }

        private void ClipboardContentListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged("SelectedItem");
            OnPropertyChanged("SelectedClipboardItems");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            base.OnClosing(e);
        }

        public ClipboardItem SelectedItem
        {
            get
            {
                if (ClipboardContentListView.SelectedItems.Count == 1)
                {
                    return ClipboardContentListView.SelectedItem as ClipboardItem;
                }
                return null;
            }
        }

        public List<ClipboardItem> SelectedClipboardItems
        {
            get
            {
                return ClipboardContentListView.SelectedItems.Cast<ClipboardItem>().ToList();
            }
        }

        public bool _isLogging = false;
        public bool IsLogging
        {
            get => _isLogging;
            set
            {
                if (_isLogging != value)
                {
                    if (value)
                    {
                        Log.TextListener += Log_TextListener;
                    }
                    else
                    {
                        Log.TextListener -= Log_TextListener;
                    }
                    _isLogging = value;
                    OnPropertyChanged("IsLogging");
                }
            }
        }

        private string _logText = "";
        public string LogText
        {
            get => _logText;
            set
            {
                _logText = value;
                OnPropertyChanged("LogText");
            }
        }

        private void Log_TextListener(string obj)
        {
            LogText += obj;
        }

        private void ClipboardItem_Copy(object sender, RoutedEventArgs e)
        {
            Clipboards.SetClipboard(SelectedClipboardItems);
        }

        private void ClipboardItem_Delete(object sender, RoutedEventArgs e)
        {
            Clipboards.RemoveItems(SelectedClipboardItems);
        }

        private void Clipboard_Clear(object sender, RoutedEventArgs e)
        {
            Clipboards.Clear();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            SaveFile();
            App.Current.Shutdown();
        }

        private void SaveFile(object sender = null, RoutedEventArgs e = null)
        {
            Controller.WriteClipboardsFile();
            Controller.WriteSettingFile();
        }

        private void LoadFile(object sender = null, RoutedEventArgs e = null)
        {
            Controller.ReadClipboardsFile();
            Controller.ReadSettingFile();
        }

        private void ShowUwpUI(object sender, RoutedEventArgs e)
        {
            Controller.Instance.ShowUwpUI();
        }

        private void DebuggerBreak(object sender, RoutedEventArgs e)
        {
            var wrapper = Clipboards.Instance.ClipboardWrapper;

            Debugger.Break();
        }
    }
}
