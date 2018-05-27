using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UWPUI
{
    internal class TextDisplayConverter : IValueConverter
    {
        public static int MaxLength = 100;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string x = value as string ?? "";
            x = System.Text.RegularExpressions.Regex.Replace(x, @"\s+", "");
            string post = x.Length > MaxLength ? x.Substring(0, MaxLength) + "......" : x;
            string pre = $"[{x.Length}]";
            return $"{pre,-7}{post}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
