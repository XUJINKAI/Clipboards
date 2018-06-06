using DataModel;
using System;
using System.IO;
using Windows.ApplicationModel.DataTransfer;
using XJK;
using XJK.Serializers;

namespace WpfBackground
{
    static class Clipboards
    {
        private static string _clipboards_folder;
        private static string _clipboards_file;

        public static event Action<ClipboardItem> Changed;
        public static bool IsListenning { get; private set; } = false;

        public static ClipboardContents Contents { get; private set; }
        
        public static void StartListen()
        {
            Log.Info("Clipboards.StartListen");
            if (IsListenning == false)
            {
                Clipboard.ContentChanged += Clipboard_ContentChanged;
            }
            IsListenning = true;
        }

        public static void StopListen()
        {
            Log.Info("Clipboards.StopListen");
            if (IsListenning == true)
            {
                Clipboard.ContentChanged -= Clipboard_ContentChanged;
            }
            IsListenning = false;
        }

        public static void Clear()
        {
            Contents.Clear();
        }

        public static void Init(string file, string folder)
        {
            _clipboards_file = file;
            _clipboards_folder = folder;
            LoadClipboardsFile();
        }

        private static void LoadClipboardsFile()
        {
            if (!File.Exists(_clipboards_file))
            {
                Contents = new ClipboardContents();
            }
            else
            {
                try
                {
                    Contents = XmlSerialization.ReadXmlFile<ClipboardContents>(_clipboards_file);
                }
                catch
                {
                    Contents = new ClipboardContents();
                    File.Delete(_clipboards_file);
                }
            }
        }

        public static void WriteToClipboards()
        {
            XmlSerialization.WriteXmlFile(Contents, _clipboards_file);
        }

        private static async void Clipboard_ContentChanged(object sender, object e)
        {
            if (IsListenning)
            {
                var data = Clipboard.GetContent();
                Log.Info("Formats: " + string.Join(", ", data.AvailableFormats));
                if (data.Contains(StandardDataFormats.Text))
                {
                    var text = await data.GetTextAsync();
                    //TODO
                    if (text.Length > 10000) { return; }
                    var item = new ClipboardItem(text);
                    Changed?.Invoke(item);
                }
                else if (data.Contains(StandardDataFormats.Bitmap))
                {
                    var streamReference = await data.GetBitmapAsync();
                    var randomAccessStreamWithContentType = await streamReference.OpenReadAsync();
                    var bytes = Converter.RandomAccessStreamToBytes(randomAccessStreamWithContentType);
                    var item = new ClipboardItem()
                    {
                        Type = ClipboardContentType.Image,
                        Base64 = bytes.ToBase64BinaryString(),
                    };
                    Changed?.Invoke(item);
                }
            }
        }
    }
}
