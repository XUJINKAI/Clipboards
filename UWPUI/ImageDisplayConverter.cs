using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using DataModel;
using XJK.Serializers;

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
            var randomstream = Converter.BytesToRandomAccessStream(bytes);
            bitmapImage.SetSource(randomstream);
            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
