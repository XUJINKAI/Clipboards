using System;
using System.Collections.Generic;
using System.Windows;
using CommonLibrary;
using DataModel;

namespace WpfBackground
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public const string AppServerName = "ClipboardsBackgroundAppService";
        public const string AppUniqueId = "com.xujinkai.clipboards.WpfBackground";
        public const string MsgConnectAppService = AppUniqueId + "_MsgConnectAppService";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (!AppHelper.IsNewInstance(AppUniqueId))
            {
                AppHelper.BroadcastMessage(MsgConnectAppService);
                Shutdown();
                return;
            }
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppHelper.RegisterAutoRestart(() =>
            {
                Shutdown();
            });
            AppHelper.RegisterReciveMessage(MsgConnectAppService, () =>
            {
                AppServiceConnect.TryConnect();
            });
            AppServer.Init();
            this.MainWindow = AppHelper.Window;
#if DEBUG
            //WpfWindow.ShowWindow();
#endif
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exp = (Exception)e.ExceptionObject;
            MessageBox.Show(exp.Message);
        }

        public static void ShutDown()
        {
            AppHelper.DisposeHelperWindow();
            AppServiceConnect.DisposeConnection();
            App.Current.Shutdown();
        }
    }
}
