using CommonLibrary;
using DataModel;
using MethodWrapper;
using MethodWrapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;
using Windows.UI.Core;

namespace UWPUI
{
    public class AppServiceInvoker : IInvokerProxy
    {
        public static AppServiceInvoker Current = new AppServiceInvoker();
        public static IClient Client => UWPUI.Client.Current;

        public static bool AutoLaunchProcess { get; set; } = false;
        
        private static BackgroundTaskDeferral AppServiceDeferral = null;
        public static AppServiceConnection Connection { get; private set; } = null;

        public static event Action Connected;
        
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
                    Connected?.Invoke();
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
                MethodCall methodCall = args.Request.Message.ToMethodCall();
                var obj = methodCall.Excute(Client);
                if (obj != null)
                {
                    MethodCall result = new MethodCall()
                    {
                        Result = obj
                    };
                    var set = result.ToValueSet();
                    var status = await args.Request.SendResponseAsync(set);
                    Log.Verbose($"Response Status {status}");
                }
            });
        }
        
        public async Task<object> InvokeAsync(MethodInfo targetMethod, object[] args)
        {
            await EnsureConnectedAsync();
            if (Connection == null)
            {
                return null;
            }
            MethodCall methodCall = new MethodCall()
            {
                Name = targetMethod.Name,
                Args = new List<object>(args),
            };
            ValueSet set = methodCall.ToValueSet();
            AppServiceResponse appServiceResponse = await Connection.SendMessageAsync(set);
            MethodCall result = appServiceResponse.Message.ToMethodCall();
            return result?.Result;
        }


        public async void EnsureConnected()
        {
            await EnsureConnectedAsync();
        }

        public async Task EnsureConnectedAsync()
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
