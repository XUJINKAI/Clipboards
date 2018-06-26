using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using XJK.AOP;
using XJK.AOP.SocketRpc;
using XJK.Network;

namespace UwpUI
{
    public class RpcInvokeProxy : SocketRpcClient
    {
        public int Port { get; private set; }

        public RpcInvokeProxy(int port)
        {
            Port = port;
            Connect("127.0.0.1", port);
        }

        public override async void DispatchInvoke(Action action)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { action(); });
        }

        protected override object GetExcuteObject()
        {
            return Controller.Instance;
        }
    }

    public class Controller : IClient
    {
        public const int DEF_PORT = 9001;

        public static IService ServerProxy { get; private set; }
        public static RpcInvokeProxy RpcInvokeProxy { get; private set; }

        public static Controller Instance { get; private set; }

        public readonly static ClipboardWrapper ClipboardWrapper;

        static Controller()
        {
            Instance = new Controller();
            ClipboardWrapper = new ClipboardWrapper();
        }

        private Controller() { }

        public static void Connect(int Port = -1)
        {
            if (Port <= 0) Port = DEF_PORT;
            try
            {
                RpcInvokeProxy = new RpcInvokeProxy(Port);
                ServerProxy = MethodProxy.CreateProxy<IService>(RpcInvokeProxy);
            }
            catch
            {
                if (Port != DEF_PORT)
                {
                    try
                    {
                        RpcInvokeProxy = new RpcInvokeProxy(DEF_PORT);
                        ServerProxy = MethodProxy.CreateProxy<IService>(RpcInvokeProxy);
                    }
                    catch
                    {
                        ConnectFail(DEF_PORT);
                    }
                }
                else
                {
                    ConnectFail(Port);
                }
            }
        }

        private static void ConnectFail(int Port)
        {
            ShowDialog($"Can't connect port {Port}");
        }
        
        public async Task RequestClipboardContentsAsync()
        {
            var wrapper = await ServerProxy.GetClipboardWrapper();
            if (wrapper != null)
            {
                ClipboardWrapper.Setting = wrapper.Setting;
                ClipboardWrapper.Clear();
                foreach (var x in wrapper.Contents)
                {
                    ClipboardWrapper.Add(x);
                }
            }
        }

        #region IClient Interface
        
        public Task ClipboardCollectionChange(List<ClipboardItem> addItems, List<ClipboardItem> removeItems)
        {
            RpcInvokeProxy.DispatchInvoke(() =>
            {
                foreach (var x in removeItems)
                {
                    ClipboardWrapper.Remove(x);
                }
                foreach (var x in addItems)
                {
                    ClipboardWrapper.Add(x);
                }
            });
            return Task.CompletedTask;
        }

        #endregion


        public static async void ShowDialog(string msg)
        {
            await ShowDialogAsync(msg);
        }
        public static async Task ShowDialogAsync(string msg)
        {
            await (new MessageDialog(msg)).ShowAsync();
        }

    }
}
