using System;
using System.Xml.Serialization;
using Windows.UI.Xaml.Media.Imaging;
using XJK.Serializers;
using static DataModel.Converter;

namespace DataModel
{
    [Serializable]
    public enum ClipboardContentType
    {
        None,
        Rtf,
        Html,
        Text,
        Image,
    }

    [Serializable]
    public class ClipboardItem
    {
        public const int TextDisplayLength = 100;
        public const int ImageDisplayMaxHeight = 100;
        public const int ImageDisplayMaxWidth = 100;

        public string FileName { get; set; }
        public DateTime Time { get; set; }

        public ClipboardContentType Type { get; set; } = ClipboardContentType.None;
        public string ContentBase64 { get; set; }
        public string TextBase64 { get; set; }
        public string ImageBase64 { get; set; }

        [XmlIgnore]
        public string StringContent
        {
            get => BinarySerialization.FromBase64BinaryString<string>(ContentBase64);
            set => ContentBase64 = value.ToBase64BinaryString();
        }

        [XmlIgnore]
        public string PureText
        {
            get => BinarySerialization.FromBase64BinaryString<string>(TextBase64);
            set => TextBase64 = value.ToBase64BinaryString();
        }

        [XmlIgnore]
        public string TextDisplay
        {
            get
            {
                string text = PureText;
                string trim = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", "");
                if (trim.Length > TextDisplayLength)
                {
                    trim = trim.Substring(0, TextDisplayLength) + "......";
                }
                string length = $"[{text.Length}]";
                return $"[{Type}]{length}{trim}";
            }
        }

        [XmlIgnore]
        public BitmapImage Image
        {
            get
            {
                if (string.IsNullOrEmpty(ImageBase64)) return null;
                return BytesToImage(ImageBytes);
            }
        }

        [XmlIgnore]
        public byte[] ImageBytes
        {
            get
            {
                if (string.IsNullOrEmpty(ImageBase64)) return null;
                return BinarySerialization.FromBase64BinaryString<byte[]>(ImageBase64);
            }
            set
            {
                ImageBase64 = value.ToBase64BinaryString();
                Type = ClipboardContentType.Image;
            }
        }
        
        public static void Resize(BitmapImage image)
        {
            var origHeight = image.PixelHeight;
            var origWidth = image.PixelWidth;
            var ratioX = ImageDisplayMaxWidth / (float)origWidth;
            var ratioY = ImageDisplayMaxHeight / (float)origHeight;
            var ratio = Math.Min(ratioX, ratioY);
            var newHeight = (int)(origHeight * ratio);
            var newWidth = (int)(origWidth * ratio);
            image.DecodePixelWidth = newWidth;
            image.DecodePixelHeight = newHeight;
        }

        public void SetText(string text)
        {
            PureText = text;
            Type = ClipboardContentType.Text;
        }
        
        public ClipboardItem()
        {
            Time = DateTime.Now;
        }

        public override bool Equals(object obj)
        {
            return obj is ClipboardItem item
                && item.Type == Type
                && item.ContentBase64 == ContentBase64
                && item.TextBase64 == TextBase64
                && item.ImageBase64 == ImageBase64;
        }

        public override int GetHashCode()
        {
            return $"[ClipboardItem]{Type}:{ContentBase64}_{TextBase64}_{ImageBase64}".GetHashCode();
        }
    }

}
