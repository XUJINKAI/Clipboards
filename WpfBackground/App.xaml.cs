using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfBackground
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static string AppUniqueId = "com.xujinkai.clipboards.WpfBackground";
        public static string MsgConnectAppService = AppUniqueId + "_MsgConnectAppService";

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
                WpfWindow.Instance.Title = "x";
            });
#if DEBUG
            WpfWindow.ShowWindow();
#endif
        }
    }
}
