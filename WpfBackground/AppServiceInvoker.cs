using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using XJK;
using XJK.AOP;
using XJK.AOP.CommunicationModel;

namespace WpfBackground
{
    public class AppServiceInvoker : AppServiceCommBase
    {
        public string AppServiceName { get; private set; }
        public string PackageFamilyName { get; private set; }
        public object ExcuteObject { get; private set; }
        
        public AppServiceInvoker(string appservername, string packagefamilyname, object excuteObj)
        {
            AppServiceName = appservername;
            PackageFamilyName = packagefamilyname;
            ExcuteObject = excuteObj;
            TryConnect(true);
        }

        public async void TryConnect(bool force = false)
        {
            await TryNewConnection(AppServiceName, PackageFamilyName, force);
        }

        public async Task TryConnectAsync(bool force = false)
        {
            await TryNewConnection(AppServiceName, PackageFamilyName, force);
        }

        protected override void DispatchInvoke(Action action)
        {
            App.Current.Dispatcher.Invoke(action);
        }

        protected override object GetExcuteObject()
        {
            return ExcuteObject;
        }

        public override void BeforeInvoke(object sender, BeforeInvokeEventArgs args)
        {
            if (!IsConnceted())
            {
                var fake = args.MethodInfo.ReturnType.DefaultValue();
                Trace.TraceInformation($"[BeforeInvoke] Not Connected, return FakeResult [{fake}]");
                args.Handle(fake);
                //var result = MessageBox.Show("Press YES try to connect...", "Not Connected", MessageBoxButton.YesNo);
                //if (result == MessageBoxResult.Yes)
                //{
                //    await TryConnectAsync();
                //}
                //else
                //{
                //    var fake = args.MethodInfo.ReturnType.DefaultValue();
                //    Trace.TraceInformation($"[BeforeInvoke] Not Connected, return FakeResult [{fake}]");
                //    args.Handle(fake);
                //}
            }
        }
    }
}
