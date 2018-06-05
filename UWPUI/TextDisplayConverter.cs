using System;
using Windows.UI.Xaml.Data;

namespace UWPUI
{
    internal class TextDisplayConverter : IValueConverter
    {
        public static int MaxLength = 100;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string x = value as string ?? "";
            string trim = System.Text.RegularExpressions.Regex.Replace(x, @"\s+", "");
            string pre = $"[{x.Length}]";
            string post = trim.Length > MaxLength ? trim.Substring(0, MaxLength) + "......" : trim;
            return $"{pre,-7}{post}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
