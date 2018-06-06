using System;
using System.IO;
using System.Windows;
using XJK;
using XJK.SysX;

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
            Log.DebugConsoleOutput = true;
            if (!AppHelper.IsNewInstance(AppUniqueId))
            {
                AppHelper.BroadcastMessage(MsgConnectAppService);
                Shutdown();
                return;
            }
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppHelper.RegisterAutoRestart(() =>
            {
                Service.Current.Shutdown();
            });
            AppHelper.RegisterReciveMessage(MsgConnectAppService, () =>
            {
                Service.AppServiceInvoker.TryConnect();
            });
            Clipboards.Init(ClipboardXmlFilePath, ClipboardContentFolder);
            Service.Init(AppServerName, PackageFamilyName);
            this.MainWindow = AppHelper.Window;
#if DEBUG
            WpfWindow.ShowWindow();
#endif
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exp = (Exception)e.ExceptionObject;
            MessageBox.Show(exp.Message);
        }
    }
}
