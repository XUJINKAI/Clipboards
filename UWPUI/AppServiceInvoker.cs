using DataModel;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using XJK;
using XJK.AOP;
using XJK.AOP.CommunicationModel;

namespace UWPUI
{
    public class AppServiceInvoker : AppServiceCommBase
    {
        public static AppServiceInvoker Current = new AppServiceInvoker();
        public static Client Client => UWPUI.Client.Current;

        public bool AutoLaunchProcess { get; set; } = false;

        private BackgroundTaskDeferral AppServiceDeferral = null;

        private AppServiceInvoker() { }
        

        public void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails)
            {
                AppServiceDeferral = args.TaskInstance.GetDeferral();
                args.TaskInstance.Canceled += OnAppServiceCanceled;
                if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails details)
                {
                    Connection = details.AppServiceConnection;
                }
            }
        }

        private void OnAppServiceCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            OnConnectionClosed(Connection, AppServiceClosedStatus.Canceled);
        }
        
        protected override void OnConnectionClosed(AppServiceConnection sender, AppServiceClosedStatus status)
        {
            AppServiceDeferral.Complete();
            base.OnConnectionClosed(sender, status);
            RetryConnectDialog();
        }

        public void RetryConnectDialog()
        {
            if (!IsConnceted())
            {
                DispatchInvoke(async () =>
                {
                    var msg = new MessageDialog("Not Connected");
                    msg.Commands.Add(new UICommand("Retry", new UICommandInvokedHandler(async (IUICommand command) =>
                    {
                        await EnsureConnectedAsync();
                    })));
                    msg.Commands.Add(new UICommand("Exit", new UICommandInvokedHandler((IUICommand command) =>
                    {
                        Client.ExitBackground();
                    })));
                    await msg.ShowAsync();
                });
            }
        }

        public override void BeforeInvoke(object sender, BeforeInvokeEventArgs args)
        {
            if (!IsConnceted())
            {
                RetryConnectDialog();
            }
        }
        
        protected override async void DispatchInvoke(Action action)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, ()=> { action(); });
        }

        protected override object GetExcuteObject()
        {
            return Client;
        }

        public static async Task EnsureConnectedAsync()
        {
            if (Current.AutoLaunchProcess && !Current.IsConnceted())
            {
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
        }
    }
}
