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
#if DEBUG
            WpfWindow.ShowWindow();
#endif
        }

        private async void TryConnect()
        {
            if (!AppServiceConnect.IsOpen)
            {
                AppServiceConnect.Received += AppServiceConnect_Received1;
                await AppServiceConnect.OpenConnection(AppServerName);
            }
        }

        private void AppServiceConnect_Received1(RequestData obj)
        {
            switch (obj.Request)
            {
                case Request.ShowWpfWindow:
                    WpfWindow.ShowWindow();
                    break;
                case Request.ShutDown:
                    Shutdown();
                    break;
                case Request.AskClipboardList:
                    AppServiceConnect.Send(new AnswerData(Request.AnsClipboardList, new ExClipboardList(Clipboards.TextList)));
                    break;
            }
        }
    }
}
