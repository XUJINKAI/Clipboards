using DataModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace WpfBackground
{
    public static class AppServiceConnect
    {
        public static bool IsOpen => _connection != null;
        public static AppServiceConnection _connection = null;

        public static event Action<AnswerData> Received;
        public static event Action Closed;

        private static void OnReceived(ValueSet set)
        {
#if DEBUG
            MessageBox.Show(set.ToAnswerData().Request.ToString());
#endif
            Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
            {
                Received?.Invoke(set.ToAnswerData());
            }));
        }

        public static void DisposeConnection()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        public static async Task<string> OpenConnection(string AppServiceName, string PackageFamilyName = null)
        {
            if (_connection != null)
            {
                DisposeConnection();
            }
            if (string.IsNullOrEmpty(PackageFamilyName))
            {
                //PackageFamilyName = Package.Current.Id.FamilyName;
                PackageFamilyName = "55774JinkaiXu.57013CAEE6225_p5dcp4q3yn5jt";
            }
            _connection = new AppServiceConnection
            {
                AppServiceName = AppServiceName,
                PackageFamilyName = PackageFamilyName,
            };
            _connection.RequestReceived += Connection_RequestReceived;
            _connection.ServiceClosed += Connection_ServiceClosed;
            AppServiceConnectionStatus status = await _connection.OpenAsync();
            if (status != AppServiceConnectionStatus.Success)
            {
                DisposeConnection();
            }
#if DEBUG
            MessageBox.Show(status.ToString());
#endif
            return status.ToString();
        }

        private static void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            DisposeConnection();
            Closed?.Invoke();
        }

        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            OnReceived(args.Request.Message);
        }

        public static async void Send(RequestData requestData)
        {
            var set = requestData.ToValueSet();
            var response = await _connection.SendMessageAsync(set);
            if (response.Message != null)
            {
                OnReceived(response.Message);
            }
#if DEBUG
            MessageBox.Show($"Request: {requestData.Request}\r\nStatus: {response.Status}");
#endif
        }
    }
}
