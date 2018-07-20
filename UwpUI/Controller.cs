using DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using XJK;
using XJK.AOP;
using XJK.AOP.SocketRpc;
using XJK.Network;
using XJK.Serializers;
using XJK.SysX;

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
        public static readonly string ApplicationDataSpecialFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static readonly string AppDataFolder = Path.Combine(ApplicationDataSpecialFolder, "Clipboards");
        public static readonly string CommXmlFileName = "comm";

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

        private static async Task<int> GetPort()
        {
            var folder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(AppDataFolder);
            var file = await folder.GetFileAsync(CommXmlFileName);
            var stream = await file.OpenStreamForReadAsync();
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();
            string xml = buffer.ConvertString();
            Comm comm = XmlSerialization.FromXmlText<Comm>(xml);
            return comm.Port;
        }

        public static async void Connect()
        {
            int Port;
            try
            {
                Port = await GetPort();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Port = DEF_PORT;
            }
            Log.Debug($"Port: {Port}");
            try
            {
                RpcInvokeProxy = new RpcInvokeProxy(Port);
                ServerProxy = MethodProxy.CreateProxy<IService>(RpcInvokeProxy);
            }
            catch (Exception ex)
            {
                await ShowDialogAsync($"Can't connect port {Port}");
            }
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
