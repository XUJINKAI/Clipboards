using AppServiceComm;
using CommonLibrary;
using DataModel;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace WpfBackground
{
    public class AppServiceCaller
    {
        public string AppServiceName { get; private set; }
        public string PackageFamilyName { get; private set; }
        public object ExcuteObject { get; private set; }

        public bool IsConnected => _connection != null;
        public AppServiceConnection _connection = null;
        
        public event Action<AppServiceClosedEventArgs> Closed;
        public event Action<AppServiceRequestReceivedEventArgs> Recived;

        public AppServiceCaller(string appservername, string packagefamilyname, object excuteObj)
        {
            AppServiceName = appservername;
            PackageFamilyName = packagefamilyname;
            ExcuteObject = excuteObj;
            TryConnect(true);
        }

        public async void TryConnect(bool force = false)
        {
            if(force && _connection != null)
            {
                DisposeConnection();
            }
            if (_connection == null)
            {
                _connection = new AppServiceConnection
                {
                    AppServiceName = AppServiceName,
                    PackageFamilyName = PackageFamilyName,
                };
                _connection.ServiceClosed += Connection_ServiceClosed;
                _connection.RequestReceived += Connection_RequestReceived;
                AppServiceConnectionStatus status = await _connection.OpenAsync();
                if (status != AppServiceConnectionStatus.Success)
                {
                    DisposeConnection();
                }
            }
        }
        
        public void DisposeConnection()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            DisposeConnection();
            Closed?.Invoke(args);
        }

        private void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            App.Current.Dispatcher.Invoke(async () =>
            {
                var func = ValueSetExtension.GetMethod(args.Request.Message);
                Log.Verbose($"Recived {func.Name}");
                var result = func.Excute(ExcuteObject);
                if (result != null)
                {
                    var response = ValueSetExtension.SetResponse(result);
                    var status = await args.Request.SendResponseAsync(response);
                    Log.Verbose($"Response Status {status}");
                }
                Recived?.Invoke(args);
            });
        }

        public async Task<object> Invoke(Expression<Func<IClient, object>> expression)
        {
            ValueSet set = ValueSetExtension.SetLambda(expression);
            AppServiceResponse appServiceResponse = await _connection.SendMessageAsync(set);
            return ValueSetExtension.GetResponse(appServiceResponse.Message);
        }

        public async void Invoke(Expression<Action<IClient>> expression)
        {
            ValueSet set = ValueSetExtension.SetLambda(expression);
            AppServiceResponse appServiceResponse = await _connection.SendMessageAsync(set);
        }
    }
}
