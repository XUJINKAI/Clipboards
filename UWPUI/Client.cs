using CommonLibrary;
using DataModel;
using MethodWrapper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace UWPUI
{
    public class Client : IClient
    {
        public readonly static Client Current;
        public static IService ServerProxy;
        public static AppServiceInvoker AppServiceInvoker => AppServiceInvoker.Current;

        public readonly static ClipboardContents ClipboardContents;

        public string LogText { get; private set; } = "";
        public event Action<string> LogChanged;

        static Client()
        {
            Current = new Client();
            ClipboardContents = new ClipboardContents();
            ServerProxy = InvokeProxy.CreateProxy<IService>(AppServiceInvoker);
            AppServiceInvoker.Connected += AppServiceInvoker_Connected;
            AppServiceInvoker.AutoLaunchProcess = true;
            AppServiceInvoker.EnsureConnected();
        }

        private static async void AppServiceInvoker_Connected()
        {
            await Current.RequestClipboardContentsAsync();
        }

        private Client()
        {
            Log.AutoStringListener += Log_AutoStringListener;
            Log.AutoFlush = true;
        }

        public static void Init()
        {

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

        private void Log_AutoStringListener(string obj)
        {
            LogText += obj;
            LogChanged?.Invoke(LogText);
        }

        public Task AddClipboardItem(ClipboardItem clipboardItem)
        {
            ClipboardContents.AddNew(clipboardItem);
            return Task.FromResult<Object>(null);
        }
    }
}
