using CommonLibrary;
using DataModel;
using MethodWrapper;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;

namespace WpfBackground
{
    public class Service : IService
    {
        public static Service Current { get; private set; }
        public static AppServiceInvoker AppServiceInvoker { get; private set; }
        public static IClient Client { get; private set; }
        
        public Service(string appservername, string packagefamilyname)
        {
            Current = this;
            AppServiceInvoker = new AppServiceInvoker(appservername, packagefamilyname, this);
            Client = InvokeProxy.CreateProxy<IClient>(AppServiceInvoker);
            Clipboards.Changed += Clipboards_Changed;
            Clipboards.StartListen();
        }

        public void Dispose()
        {
            if (AppServiceInvoker != null)
            {
                AppServiceInvoker.DisposeConnection();
            }
        }

        private void Clipboards_Changed(string text)
        {
            Client.AddClipboardItem(new ClipboardItem(text));
        }


        public Task ShowUwpWindow()
        {
            WpfWindow.ShowWindow();
            return Task.FromResult<object>(null);
        }

        public Task Shutdown()
        {
            App.ShutDown();
            return Task.FromResult<object>(null);
        }
        
        public Task<ClipboardContents> GetClipboardContents()
        {
            return Task.FromResult(Clipboards.Contents);
        }

        public Task<bool> ClearClipboardList()
        {
            Clipboards.Clear();
            return Task.FromResult(true);
        }

        public Task<Setting> GetSetting()
        {
            return Task.FromResult(new Setting());
        }

        public Task<bool> SetTopmost(bool topmost)
        {
            IntPtr handle = Topmost.GetForegroundWindow();
            Topmost.SetOrToggle(handle, topmost);
            var result = Topmost.Get(handle);
            return Task.FromResult(result);
        }

        public Task SetClipboard(ClipboardItem clipboardItem)
        {
            Clipboard.SetDataObject(clipboardItem.Text);
            return Task.FromResult<object>(null);
        }
    }
}
