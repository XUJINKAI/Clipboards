using CommonLibrary;
using DataModel;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace WpfBackground
{
    public static class AppServiceConnect
    {
        public static bool IsConnected => _connection != null;
        public static AppServiceConnection _connection = null;

        public static event Action<ConnectionData> Received;
        public static event Action Closed;

        private static void OnReceived(ValueSet set)
        {
            var data = set.ToConnectionData();
            App.Current.Dispatcher.Invoke(() =>
            {
                Received?.Invoke(data);
            });
        }

        public static async void TryConnect()
        {
            if (!IsConnected)
            {
                await ConnectionAsync(App.AppServerName);
            }
        }

        public static async Task<string> ConnectionAsync(string AppServiceName, string PackageFamilyName = null)
        {
            if (_connection != null)
            {
                DisposeConnection();
            }
            Log.Info("Connecting");
            if (string.IsNullOrEmpty(PackageFamilyName))
            {
                PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;
                //PackageFamilyName = "55774JinkaiXu.57013CAEE6225_p5dcp4q3yn5jt";
            }
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
            Log.Info($"OpenConnection {status}");
            return status.ToString();
        }

        public static void DisposeConnection()
        {
            Log.Verbose($"Dispose Connection");
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        private static void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Log.Verbose("AppServiceConnect_Closed");
            DisposeConnection();
            Closed?.Invoke();
        }

        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            OnReceived(args.Request.Message);
        }

        public static async void Send(ConnectionData requestData)
        {
            if (IsConnected)
            {
                var set = requestData.ToValueSet();
                var response = await _connection.SendMessageAsync(set);
                if (response.Message != null)
                {
                    OnReceived(response.Message);
                }
                Log.Verbose($"Send Request: {requestData.Command}\r\nStatus: {response.Status}");
            }
        }
    }
}
