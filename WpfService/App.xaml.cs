using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfService.Model;
using XJK;
using XJK.WPF;

namespace WpfService
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
#if DEBUG
        private const bool _debug = true;
#else
        private const bool _debug = false;
#endif
        public static bool DEBUG => _debug;

        public const string InstanceId = "Clipboards_WpfService_SingleId";
        public const string ShowWpfUiMsg = "Clipboards_WpfService_ShowWpfUi";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (!SingleInstance.IsNewInstance(InstanceId))
            {
                WinMsg.BroadcastMessage(ShowWpfUiMsg);
                Shutdown();
                return;
            }
            WinMsg.RegisterReciveMessage(ShowWpfUiMsg, () =>
            {
                this.MainWindow.Show();
            });
            Log.LogLocation = true;
            Log.ListenSystemDiagnosticsLog();
            Clipboards.Instance.IsListenning = true;
            this.MainWindow = new MainWindow();
            this.MainWindow.Show();
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
    }
}
