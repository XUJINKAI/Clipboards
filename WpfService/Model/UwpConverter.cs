using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using XJK.Serializers;
using static WpfService.Model.RandomAccessStreamConverter;

namespace WpfService
{
    public static class UwpConverter
    {
        public static BitmapImage BytesToUwpBitmapImage(byte[] bytes)
        {
            BitmapImage bitmapImage = new BitmapImage();
            if (bytes == null) return null;
            var randomstream = BytesToRandomAccessStream(bytes);
            bitmapImage.SetSource(randomstream);
            return bitmapImage;
        }

        public static byte[] UwpBitmapImageToBytes(BitmapImage image)
        {
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            async Task<byte[]> task_func()
            {
                await image.SetSourceAsync(randomAccessStream);
                var bytes = new byte[randomAccessStream.Size];
                await randomAccessStream.ReadAsync(bytes.AsBuffer(), (uint)randomAccessStream.Size, InputStreamOptions.ReadAhead);
                return bytes;
            }
            var task = task_func();
            Task.Run(async () => { await task; });
            return task.Result;
        }

        public static string SerializeUwpBitmapImage(BitmapImage image)
        {
            return (UwpBitmapImageToBytes(image)).ToBase64BinaryString();
        }

        public static BitmapImage DeserializeToUwpBitmapImage(string serialized)
        {
            return BytesToUwpBitmapImage(BinarySerialization.FromBase64BinaryString<byte[]>(serialized));
        }
    }
}
