using CommonLibrary;
using DataModel;
using MethodWrapper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Popups;

namespace UWPUI
{
    public class Client : IClient
    {
        public readonly static Client Current;
        public static IService ServerProxy;
        public static AppServiceInvoker AppServiceInvoker => AppServiceInvoker.Current;

        public readonly static ClipboardContents ClipboardContents;
        
        public bool IsTopmost { get; private set; }
        public event Action<bool> TopmostChanged;

        static Client()
        {
            Current = new Client();
            ClipboardContents = new ClipboardContents();
            ServerProxy = MethodProxy.CreateProxy<IService>(AppServiceInvoker);
            AppServiceInvoker.Connected += AppServiceInvoker_Connected;
            AppServiceInvoker.AutoLaunchProcess = true;
        }

        private static async void AppServiceInvoker_Connected()
        {
            await Current.RequestClipboardContentsAsync();
            await Current.SetTopmostByTitleAsync(true);
        }

        private Client()
        {
            Log.Prefix = "[ClipboardsUwp]";
            Log.AddDebugConsoleListener();
            Log.AutoFlush = true;
        }

        public async void Init()
        {
            await AppServiceInvoker.EnsureConnectedAsync();
        }

        public static async Task ShowMsgAsync(string msg)
        {
            await (new MessageDialog(msg).ShowAsync());
        }

        public async void ExitBackground()
        {
            App.Current.Exit();
            AppServiceInvoker.AutoLaunchProcess = false;
            if (AppServiceInvoker.Connection != null)
            {
                await ServerProxy.Shutdown();
            }
        }

        public async Task SetTopmostAsync(bool topmost)
        {
            var result = await ServerProxy.SetTopmost(topmost);
            IsTopmost = result;
            TopmostChanged?.Invoke(result);
        }
        
        public async Task SetTopmostByTitleAsync(bool topmost)
        {
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            string pastTitle = appView.Title;
            appView.Title = Helper.RandomString(30);
            var result = await ServerProxy.SetTopmostByTitle(appView.Title, topmost);
            appView.Title = pastTitle;
            IsTopmost = result;
            TopmostChanged?.Invoke(result);
        }

        public async Task RequestClipboardContentsAsync()
        {
            var list = await ServerProxy.GetClipboardContents();
            ClipboardContents.Clear();
            if (list != null)
            {
                foreach (var x in list)
                {
                    ClipboardContents.Add(x);
                }
            }
        }
        
        public Task AddClipboardItem(ClipboardItem clipboardItem)
        {
            ClipboardContents.AddNew(clipboardItem);
            return Task.FromResult<Object>(null);
        }
    }
}
