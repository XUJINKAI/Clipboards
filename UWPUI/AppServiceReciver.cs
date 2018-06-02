using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace UWPUI
{
    public static class AppServiceReciver
    {
        private static BackgroundTaskDeferral AppServiceDeferral = null;

        public static AppServiceConnection Connection { get; private set; } = null;
        public static IClient Client { get; private set; } = null;

        public static event Action ConnectFailed;
        public static bool AutoLaunchProcess { get; set; } = false;

        public static async void Init(IClient client, bool auto_launch)
        {
            Client = client;
            AutoLaunchProcess = auto_launch;
            await EnsureConnected();
        }

        public static void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails)
            {
                AppServiceDeferral = args.TaskInstance.GetDeferral();
                args.TaskInstance.Canceled += OnAppServiceCanceled;
                if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails details)
                {
                    Connection = details.AppServiceConnection;
                    Connection.ServiceClosed += Connection_ServiceClosed;
                    Connection.RequestReceived += OnConnection_RequestReceived;
                }
            }
        }

        private static void OnAppServiceCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            OnConnectionClosed();
        }

        private static void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            OnConnectionClosed();
        }

        private static async void OnConnectionClosed()
        {
            if (AppServiceDeferral != null)
            {
                AppServiceDeferral.Complete();
            }
            Connection.Dispose();
            Connection = null;
            if (AutoLaunchProcess)
            {
                await LaunchBackgroundProcessAsync();
            }
        }

        private static async void OnConnection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var method = ValueSetExtension.GetMethod(args.Request.Message);
                var obj = method.Excute(Client);
                if (obj != null)
                {
                    var set = ValueSetExtension.SetResponse(obj);
                    var status = await args.Request.SendResponseAsync(set);
                }
            });
        }

        public static async Task<object> Invoke(Expression<Func<IService, object>> expression)
        {
            await EnsureConnected();
            if (Connection == null)
            {
                ConnectFailed?.Invoke();
                return null;
            }
            var set = ValueSetExtension.SetLambda(expression);
            var result = await Connection.SendMessageAsync(set);
            var obj = ValueSetExtension.GetResponse(result.Message);
            return obj;
        }

        public static async Task Invoke(Expression<Action<IService>> expression)
        {
            await EnsureConnected();
            if (Connection == null)
            {
                ConnectFailed?.Invoke();
                return;
            }
            var set = ValueSetExtension.SetLambda(expression);
            await Connection.SendMessageAsync(set);
        }

        public static async Task EnsureConnected()
        {
            if (AutoLaunchProcess && Connection == null)
            {
                await LaunchBackgroundProcessAsync();
            }
        }

        public static async Task LaunchBackgroundProcessAsync()
        {
            await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
        }

    }
}
