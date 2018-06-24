using System;
using System.Xml.Serialization;
using XJK;
using XJK.Serializers;

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
        public override int GetHashCode()
        {
            return $"[ClipboardItem]{Type}:{ContentBase64}_{TextBase64}_{ImageBase64}".GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is ClipboardItem item
                && item.Type == Type
                && item.ContentBase64 == ContentBase64
                && item.TextBase64 == TextBase64
                && item.ImageBase64 == ImageBase64;
        }

        public int MaxSizeBytes
        {
            get
            {
                int stringsize = System.Text.Encoding.ASCII.GetByteCount(StringContent);
                int imagesize = ImageBytes.Length;
                return Math.Max(stringsize, imagesize);
            }
        }

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
            get
            {
                if (ContentBase64.IsNullOrEmpty()) return "";
                else return BinarySerialization.FromBase64BinaryString<string>(ContentBase64);
            }
            set => ContentBase64 = value.ToBase64BinaryString();
        }

        [XmlIgnore]
        public string PureText
        {
            get
            {
                if (TextBase64.IsNullOrEmpty()) return "";
                else return BinarySerialization.FromBase64BinaryString<string>(TextBase64);
            }
            set => TextBase64 = value.ToBase64BinaryString();
        }

        [XmlIgnore]
        public int PureTextLength => PureText.Length;

        [XmlIgnore]
        public string TextDisplay
        {
            get
            {
                string display = $"[{Type}]";
                if (ImageBytes.Length > 0)
                {
                    display += $"[{ImageBytes.Length / 1024}kb]";
                }
                string text = PureText;
                string trim = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", "");
                if (trim.Length > TextDisplayLength)
                {
                    trim = trim.Substring(0, TextDisplayLength) + "......";
                }
                string length = $"[{text.Length}]";
                display += $"{length}{trim}";
                return display;
            }
        }
        
        [XmlIgnore]
        public byte[] ImageBytes
        {
            get
            {
                if (string.IsNullOrEmpty(ImageBase64)) return new byte[0];
                return BinarySerialization.FromBase64BinaryString<byte[]>(ImageBase64);
            }
            set
            {
                ImageBase64 = value.ToBase64BinaryString();
                Type = ClipboardContentType.Image;
            }
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

    }

}
