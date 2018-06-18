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
using XJK.AOP.AppServiceRpc;

namespace UWPUI
{
    public class AppServiceInvoker : AppServiceBase
    {
        private BackgroundTaskDeferral AppServiceDeferral = null;

        public void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails)
            {
                AppServiceDeferral = args.TaskInstance.GetDeferral();
                args.TaskInstance.Canceled += TaskInstance_Canceled;
                if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails details)
                {
                    Connection = details.AppServiceConnection;
                    Log.Info("Connected [OnBackgroundActivated]");
                }
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            OnConnectionClosed(AppServiceClosedStatus.Canceled);
        }

        protected override void OnConnectionClosed(AppServiceClosedStatus status)
        {
            base.OnConnectionClosed(status);
            if (!App.UiLaunched)
            {
                AppServiceDeferral.Complete();
            }
        }
        
        protected override async void DispatchInvoke(Action action)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, ()=> { action(); });
        }

        protected override object GetExcuteObject()
        {
            return Client.Current;
        }

        public async Task EnsureConnectedAsync()
        {
            if (!IsConnceted())
            {
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
        }

        protected override void OnInvokeNoConnection()
        {
            RetryDialog();
        }

        protected void RetryDialog()
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
                    Client.Current.ExitBackground();
                })));
                await msg.ShowAsync();
            });
        }
    }
}
