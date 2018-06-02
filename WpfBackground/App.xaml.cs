using CommonLibrary;
using System;
using System.IO;
using System.Windows;

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

        public static readonly string ApplicationDataSpecialFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string AppDataFolder = FS.CreateFolder(Path.Combine(ApplicationDataSpecialFolder, "Clipboards"));
        public static readonly string ClipboardsFolder = FS.CreateFolder(Path.Combine(AppDataFolder, "Clipboards"));
        public static readonly string ClipboardsXmlFilePath = Path.Combine(AppDataFolder, "Clipboards.xml");

        private static Service Service;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (!AppHelper.IsNewInstance(AppUniqueId))
            {
                AppHelper.BroadcastMessage(MsgConnectAppService);
                Shutdown();
                return;
            }
            Log.Prefix = $"[{Module.ModuleHandleHex}]";
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppHelper.RegisterAutoRestart(() =>
            {
                Shutdown();
            });
            Clipboards.Init(ClipboardsXmlFilePath, ClipboardsFolder);
            Service = new Service(AppServerName, PackageFamilyName);
            AppHelper.RegisterReciveMessage(MsgConnectAppService, () =>
            {
                Service.AppServiceCaller.TryConnect();
            });
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

        public static void ShutDown()
        {
            Clipboards.WriteToClipboards();
            AppHelper.DisposeHelperWindow();
            Service.Dispose();
            App.Current.Shutdown();
        }
    }
}
