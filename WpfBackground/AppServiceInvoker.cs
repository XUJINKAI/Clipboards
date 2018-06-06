using System;
using XJK.CommunicationModel;

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
        
        protected override void DispatchInvoke(Action action)
        {
            App.Current.Dispatcher.Invoke(action);
        }

        protected override object GetExcuteObject()
        {
            return ExcuteObject;
        }
    }
}
