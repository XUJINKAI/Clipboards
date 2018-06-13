using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using XJK;
using XJK.SysX;
using XJK.WPF;

namespace WpfBackground
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public const string AppServerName = "ClipboardsBackgroundAppService";
        public const string PackageFamilyName = "55774JinkaiXu.57013CAEE6225_p5dcp4q3yn5jt";
        public const string AppUniqueId = "com.xujinkai.clipboards.WpfBackground";
        public const string MsgConnectAppService = AppUniqueId + "_MsgConnectAppService";

        public static readonly string ApplicationDataSpecialFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static readonly string AppDataFolder = FS.CreateFolder(Path.Combine(ApplicationDataSpecialFolder, "Clipboards"));
        public static readonly string SettingXmlFilePath = Path.Combine(AppDataFolder, "Setting.xml");
        public static readonly string ClipboardXmlFilePath = Path.Combine(AppDataFolder, "Clipboards.xml");
        public static readonly string ClipboardContentFolder = FS.CreateFolder(Path.Combine(AppDataFolder, "ClipboardContents"));
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Log.ListenSystemDiagnosticsLog();
            if (!SingleInstance.IsNewInstance(AppUniqueId))
            {
                WinMsg.BroadcastMessage(MsgConnectAppService);
                Shutdown();
                return;
            }
            WinMsg.RegisterAutoRestart(() =>
            {
                Service.Current.Shutdown();
            });
            WinMsg.RegisterReciveMessage(MsgConnectAppService, () =>
            {
                Service.AppServiceInvoker.TryConnect();
            });
            Clipboards.Init(ClipboardXmlFilePath, ClipboardContentFolder);
            Service.Init(AppServerName, PackageFamilyName);
            this.MainWindow = WinMsg.Window;
#if DEBUG
            WpfWindow.ShowWindow();
#endif
        }
    }
}
