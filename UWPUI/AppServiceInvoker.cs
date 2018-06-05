using CommonLibrary;
using CommonLibrary.Extensions;
using DataModel;
using MethodWrapper;
using MethodWrapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
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

        private static readonly ManualResetEvent BackgroundLaunchedSignal = new ManualResetEvent(false);

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
                    Log.Debug("Connected (OnBackgroundActivated)");
                    BackgroundLaunchedSignal.Set();
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
            Log.Debug("ConnectionClosed");
            BackgroundLaunchedSignal.Reset();
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
                MethodCallInfo methodCall = args.Request.Message.ToMethodCall();
                Log.Debug($"[Reveive] {methodCall.Name}");
                object obj;
                try
                {
                    obj = methodCall.Excute(Client);
                    Log.Debug($"[ExcuteResult] {obj}");
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.ToStringLong());
                    obj = null;
                }
                if (obj != null)
                {
                    Log.Debug("[SendingResponse]");
                    MethodCallInfo result = new MethodCallInfo()
                    {
                        Name = methodCall.Name,
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
            if (Connection == null)
            {
                await UWPUI.Client.ShowMsgAsync("Not Connected");
                if (targetMethod.ReturnType.IsValueType)
                {
                    return Activator.CreateInstance(targetMethod.ReturnType);
                }
                return null;
            }
            MethodCallInfo methodCall = new MethodCallInfo()
            {
                Name = targetMethod.Name,
                Args = new List<object>(args),
            };
            ValueSet set = methodCall.ToValueSet();
            Log.Debug($"[Invoke] {methodCall}");
            AppServiceResponse appServiceResponse = await Connection.SendMessageAsync(set);
            Log.Debug($"[AppServiceResponse] {appServiceResponse}");
            MethodCallInfo result = appServiceResponse.Message.ToMethodCall();
            Log.Debug($"[Result] {result}");
            return result?.Result;
        }
        
        public async Task EnsureConnectedAsync()
        {
            if (AutoLaunchProcess && Connection == null)
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
