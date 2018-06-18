using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel.AppService;
using XJK;
using XJK.AOP;
using XJK.AOP.AppServiceRpc;

namespace WpfBackground
{
    public class AppServiceInvoker : AppServiceClient
    {
        public AppServiceInvoker(string packageName, string serverName) : base(packageName, serverName)
        {
        }

        protected override async void OnInvokeNoConnection()
        {
            await Connect();
        }
        
        protected override void DispatchInvoke(Action action)
        {
            App.Current.Dispatcher.Invoke(action);
        }

        protected override object GetExcuteObject()
        {
            return Service.Current;
        }
    }
}
