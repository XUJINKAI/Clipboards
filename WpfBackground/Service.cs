using CommonLibrary;
using DataModel;
using MethodWrapper;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

namespace WpfBackground
{
    public class Service : IService
    {
        public static Service Current { get; private set; }
        public static AppServiceInvoker AppServiceInvoker { get; private set; }
        public static IClient ClientProxy { get; private set; }
        
        public Service(string appservername, string packagefamilyname)
        {
            Current = this;
            AppServiceInvoker = new AppServiceInvoker(appservername, packagefamilyname, this);
            ClientProxy = MethodProxy.CreateProxy<IClient>(AppServiceInvoker);
            Clipboards.Changed += Clipboards_Changed;
            Clipboards.StartListen();
        }

        public void Dispose()
        {
            if (AppServiceInvoker != null)
            {
                AppServiceInvoker.DisposeConnection();
            }
        }

        private void Clipboards_Changed(ClipboardItem item)
        {
            ClientProxy.AddClipboardItem(item);
        }


        public Task ShowUwpWindow()
        {
            WpfWindow.ShowWindow();
            return Task.FromResult<object>(null);
        }

        public Task Shutdown()
        {
            App.ShutDown();
            return Task.FromResult<object>(null);
        }
        
        public Task<ClipboardContents> GetClipboardContents()
        {
            return Task.FromResult(Clipboards.Contents);
        }

        public Task<bool> ClearClipboardList()
        {
            Clipboards.Clear();
            return Task.FromResult(true);
        }

        public Task<Setting> GetSetting()
        {
            return Task.FromResult(new Setting());
        }

        public Task<bool> SetTopmost(bool topmost)
        {
            IntPtr handle = Handle.GetActiveWindow();
            Topmost.SetOrToggle(handle, topmost);
            var result = Topmost.Get(handle);
            return Task.FromResult(result);
        }

        public Task<bool> SetTopmostByTitle(string Title, bool topmost)
        {
            IntPtr hwnd = Handle.GetByTitle(Title);
            Topmost.SetOrToggle(hwnd, topmost);
            var result = Topmost.Get(hwnd);
            return Task.FromResult(result);
        }

        public async Task SetClipboard(ClipboardItem clipboardItem)
        {
            DataPackage dataPackage;
            switch (clipboardItem.Type)
            {
                case ClipboardContentType.Text:
                    dataPackage = new DataPackage();
                    dataPackage.SetText(clipboardItem.Text);
                    Clipboard.SetContent(dataPackage);
                    break;
                case ClipboardContentType.Image:
                    var bytes = BinarySerialization.FromBase64BinaryString<byte[]>(clipboardItem.Base64);
                    dataPackage = new DataPackage();
                    var randomStream = await ConvertTo(bytes);
                    dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(randomStream));
                    Clipboard.SetContent(dataPackage);
                    break;
            }
        }

        internal static async Task<InMemoryRandomAccessStream> ConvertTo(byte[] arr)
        {
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            await randomAccessStream.WriteAsync(arr.AsBuffer());
            randomAccessStream.Seek(0);
            return randomAccessStream;
        }

    }
}
