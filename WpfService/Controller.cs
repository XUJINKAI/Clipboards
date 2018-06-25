using DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfService.Model;
using XJK;
using XJK.AOP;
using XJK.AOP.SocketRpc;
using XJK.Network;
using XJK.Network.Socket;
using XJK.Serializers;
using XJK.SysX;

namespace WpfService
{
    public class RpcServerProxy : SocketRpcServer
    {
        public int Port { get; private set; }

        public RpcServerProxy(int port)
        {
            Port = port;
            Serve("127.0.0.1", port);
        }

        public override void DispatchInvoke(Action action)
        {
            App.Current.Dispatcher.Invoke(action);
        }

        protected override object GetExcuteObject()
        {
            return Controller.Instance;
        }
    }

    public class Controller : IService
    {
        public static IClient ClientProxy { get; private set; }
        public static RpcServerProxy RpcServerProxy { get; private set; }

        public static Controller Instance { get; private set; }

        static Controller()
        {
            Instance = new Controller();
            int Port = NetHelper.GetAvailablePort(9000);
            RpcServerProxy = new RpcServerProxy(Port);
            ClientProxy = MethodProxy.CreateProxy<IClient>(RpcServerProxy);
        }

        private Controller() { }

        #region IService
        public Task<bool> ClearClipboardList()
        {
            Clipboards.Clear();
            return Task.FromResult(true);
        }

        public Task<ClipboardWrapper> GetClipboardWrapper()
        {
            return Task.FromResult(Clipboards.Instance.ClipboardWrapper);
        }

        public Task<Setting> GetSetting()
        {
            return Task.FromResult(new Setting());
        }

        public Task SetClipboard(ClipboardItem clipboardItem)
        {
            RpcServerProxy.DispatchInvoke(() =>
            {
                Clipboards.SetClipboard(clipboardItem);
            });
            return Task.CompletedTask;
        }

        public Task SetClipboard(List<ClipboardItem> list)
        {
            RpcServerProxy.DispatchInvoke(() =>
            {
                Clipboards.SetClipboard(list);
            });
            return Task.CompletedTask;
        }

        public Task<bool> SetSetting(Setting setting)
        {
            return Task.FromResult(true);
        }

        public Task ShowUwpWindow()
        {
            App.Current.MainWindow.Show();
            return Task.CompletedTask;
        }

        public Task Shutdown()
        {
            RpcServerProxy.DispatchInvoke(() =>
            {
                App.Current.Shutdown();
            });
            return Task.CompletedTask;
        }

        public Task WriteDataFile()
        {
            RpcServerProxy.DispatchInvoke(() =>
            {
                WriteClipboardsFile();
                WriteSettingFile();
            });
            return Task.CompletedTask;
        }
        #endregion

        public static readonly string ApplicationDataSpecialFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static readonly string AppDataFolder = FS.CreateFolder(Path.Combine(ApplicationDataSpecialFolder, "Clipboards"));
        public static readonly string SettingXmlFilePath = Path.Combine(AppDataFolder, "Setting.xml");
        public static readonly string ClipboardXmlFilePath = Path.Combine(AppDataFolder, "Clipboards.xml");
        public static readonly string ClipboardContentFolder = FS.CreateFolder(Path.Combine(AppDataFolder, "ClipboardContents"));

        public static void ReadClipboardsFile()
        {
            if (File.Exists(ClipboardXmlFilePath))
            {
                string xml = "";
                try
                {
                    xml = FS.ReadAllText(ClipboardXmlFilePath);
                    var wrapper = XmlSerialization.FromXmlText<ClipboardWrapper>(xml);
                    Clipboards.Instance.ClipboardWrapper = wrapper;
                }
                catch (Exception ex)
                {
                    Log.Write("Parse Clipboards XML Error");
                    Log.Error(ex);
                }
            }
        }

        public static void WriteClipboardsFile()
        {
            Clipboards.Instance.ClipboardWrapper.ToXmlText().WriteToAll(ClipboardXmlFilePath);
        }

        public static void ReadSettingFile()
        {
            if (File.Exists(SettingXmlFilePath))
            {
                string xml = "";
                try
                {
                    xml = FS.ReadAllText(SettingXmlFilePath);
                    var setting = XmlSerialization.FromXmlText<Setting>(xml);
                }
                catch (Exception ex)
                {
                    Log.Write("Parse Clipboards XML Error");
                    Log.Error(ex);
                }
            }
        }

        public static void WriteSettingFile()
        {
            Setting setting = new Setting()
            {

            };
            setting.ToXmlText().WriteToAll(SettingXmlFilePath);
        }
    }
}
