using DataModel;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using XJK;
using XJK.CommModel;

namespace UWPUI
{
    public class AppServiceInvoker : AppServiceCommBase
    {
        public static AppServiceInvoker Current = new AppServiceInvoker();
        public static IClient Client => UWPUI.Client.Current;

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
        
        protected override async void OnConnectionClosed(AppServiceConnection sender, AppServiceClosedStatus status)
        {
            base.OnConnectionClosed(sender, status);
            if (AutoLaunchProcess)
            {
                await LaunchBackgroundProcessAsync();
            }
            else
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
            return Client;
        }

        public static async Task EnsureConnectedAsync()
        {
            if (Current.AutoLaunchProcess && !Current.IsConnceted())
            {
                Log.Debug("LaunchBackgroundProcessAsync start");
                await LaunchBackgroundProcessAsync();
                Log.Debug("LaunchBackgroundProcessAsync end");
            }
        }

        public static async Task LaunchBackgroundProcessAsync()
        {
            await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
        }

    }
}
