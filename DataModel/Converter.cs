using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media.Imaging;
using XJK.Serializers;

namespace DataModel
{
    public static class Converter
    {
        public static InMemoryRandomAccessStream BytesToRandomAccessStream(byte[] bytes)
        {
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            async Task task_func(byte[] bs)
            {
                await randomAccessStream.WriteAsync(bs.AsBuffer());
            };
            var task = task_func(bytes);
            Task.Run(async () => { await task; });
            randomAccessStream.Seek(0);
            return randomAccessStream;
        }

        public static byte[] RandomAccessStreamToBytes(IRandomAccessStreamWithContentType randomAccessStream)
        {
            async Task<byte[]> task_func(IRandomAccessStreamWithContentType ras)
            {
                var reader = new DataReader(ras.GetInputStreamAt(0));
                await reader.LoadAsync((uint)ras.Size);
                var bytes = new byte[ras.Size];
                reader.ReadBytes(bytes);
                return bytes;
            };
            var task = task_func(randomAccessStream);
            Task.Run(async () => { await task; });
            return task.Result;
        }

        public static BitmapImage BytesToImage(byte[] bytes)
        {
            BitmapImage bitmapImage = new BitmapImage();
            if (bytes == null) return null;
            var randomstream = BytesToRandomAccessStream(bytes);
            bitmapImage.SetSource(randomstream);
            return bitmapImage;
        }

        public static byte[] ImageToBytes(BitmapImage image)
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

        public static string SerializeImage(BitmapImage image)
        {
            return (ImageToBytes(image)).ToBase64BinaryString();
        }

        public static BitmapImage DeserializeToImage(string serialized)
        {
            return BytesToImage(BinarySerialization.FromBase64BinaryString<byte[]>(serialized));
        }
    }
}
