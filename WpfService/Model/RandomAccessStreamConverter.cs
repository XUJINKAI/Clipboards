using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace WpfService.Model
{
    public static class RandomAccessStreamConverter
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
    }
}
