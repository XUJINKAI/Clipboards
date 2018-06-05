using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPUI
{
    internal class Base64ImageDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BitmapImage bitmapImage = new BitmapImage();
            string base64 = value as string ?? "";
            if (string.IsNullOrEmpty(base64)) return bitmapImage;
            byte[] bytes = BinarySerialization.FromBase64BinaryString<byte[]>(base64);
            var task = ConvertTo(bytes);
            Task.Run(async()=> { await task; });
            var randomstream = task.Result;
            bitmapImage.SetSource(randomstream);
            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
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
