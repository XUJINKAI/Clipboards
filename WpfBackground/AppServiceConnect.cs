using DataModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace WpfBackground
{
    public static class AppServiceConnect
    {
        public static bool IsOpen => _connection != null;
        public static AppServiceConnection _connection = null;

        public static event Action<RequestData> Received;
        public static event Action Closed;

        private static void OnReceived(ValueSet set)
        {
            Received?.Invoke(new RequestData(set["Request"] as string, set["Data"] as string));
        }

        public static void DisposeConnection()
        {
            _connection.Dispose();
            _connection = null;
        }

        public static async Task<string> OpenConnection(string AppServiceName, string PackageFamilyName = null)
        {
            if (_connection != null)
            {
                DisposeConnection();
            }
            if (string.IsNullOrEmpty(PackageFamilyName))
            {
#if DEBUG
                PackageFamilyName = "FakePackageFamilyName";
#else
                PackageFamilyName = Package.Current.Id.FamilyName;
#endif
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

        public static async void Send(AnswerData answerData)
        {
            var set = new ValueSet
            {
                { "Request", answerData.Request.ToString() },
                { "Data", answerData.Data.ToXmlText() }
            };

            var response = await _connection.SendMessageAsync(set);
            if (response.Message != null)
            {
                OnReceived(response.Message);
            }
        }
    }
}
