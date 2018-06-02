using CommonLibrary;
using DataModel;
using System;
using System.Linq.Expressions;
using System.Windows;

namespace WpfBackground
{
    public class Service : IService
    {
        public static Service Instance { get; private set; }

        public AppServiceCaller AppServiceCaller { get; private set; }

        public Service(string appservername, string packagefamilyname)
        {
            AppServiceCaller = new AppServiceCaller(appservername, packagefamilyname, this);
            Clipboards.Changed += Clipboards_Changed;
            Clipboards.StartListen();
            Instance = this;
        }

        public void Dispose()
        {
            if (AppServiceCaller != null)
            {
                AppServiceCaller.DisposeConnection();
            }
        }

        private void Clipboards_Changed(string text)
        {
            AppServiceCaller.Invoke(c => c.AddClipboardItem(new ClipboardItem(text)));
        }
        
       
        public void ShowUwpWindow()
        {
            WpfWindow.ShowWindow();
        }

        public void Shutdown()
        {
            App.ShutDown();
        }

        public ClipboardContents GetClipboardContents()
        {
            return Clipboards.Contents;
        }

        public bool ClearClipboardList()
        {
            Clipboards.Clear();
            return true;
        }

        public Setting GetSetting()
        {
            return new Setting();
        }

        public bool SetTopmost(bool topmost)
        {
            IntPtr handle = Topmost.GetForegroundWindow();
            Topmost.SetOrToggle(handle, topmost);
            return Topmost.Get(handle);
        }

        public bool SetClipboard(ClipboardItem clipboardItem)
        {
            Clipboard.SetDataObject(clipboardItem.Text);
            return true;
        }
    }
}
