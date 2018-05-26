using System;
using System.Collections.Generic;
using System.Windows;
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
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            AppHelper.RegisterAutoRestart(()=>
            {
                Shutdown();
            });
            AppHelper.RegisterReciveMessage(MsgConnectAppService, () =>
            {
                TryConnect();
            });
            Clipboards.StartListen();
            TryConnect();
            Clipboards.Changed += Clipboards_Changed;
#if DEBUG
            WpfWindow.ShowWindow();
#endif
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exp = (Exception)e.ExceptionObject;
            MessageBox.Show(exp.Message);
        }

        private void Clipboards_Changed()
        {
            if (AppServiceConnect.IsOpen)
            {
                AppServiceConnect.Send(new RequestData(Request.AnsClipboardList, new ExClipboardList(Clipboards.TextList)));
            }
        }

        private async void TryConnect()
        {
            if (!AppServiceConnect.IsOpen)
            {
                AppServiceConnect.Received += AppServiceConnect_Received;
                AppServiceConnect.Closed += AppServiceConnect_Closed;
                await AppServiceConnect.OpenConnection(AppServerName);
            }
        }

        private void AppServiceConnect_Closed()
        {
            AppServiceConnect.Received -= AppServiceConnect_Received;
            AppServiceConnect.Closed -= AppServiceConnect_Closed;
            TryConnect();
        }

        private void AppServiceConnect_Received(AnswerData obj)
        {
            switch (obj.Request)
            {
                case Request.ShowWpfWindow:
                    WpfWindow.ShowWindow();
                    break;
                case Request.ShutDown:
                    ShutDown();
                    break;
                case Request.AskClipboardList:
                    AppServiceConnect.Send(new RequestData(Request.AnsClipboardList, new ExClipboardList(Clipboards.TextList)));
                    break;
            }
        }

        public static void ShutDown()
        {
            AppServiceConnect.DisposeConnection();
            App.Current.Shutdown();
        }
    }
}
