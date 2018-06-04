using CommonLibrary;
using DataModel;
using System;
using System.IO;
using Windows.ApplicationModel.DataTransfer;

namespace WpfBackground
{
    static class Clipboards
    {
        private static string _clipboards_folder;
        private static string _clipboards_file;

        public static event Action<string> Changed;
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
            Contents = XmlSerialization.ReadXmlFile<ClipboardContents>(_clipboards_file, true);
        }

        public static void WriteToClipboards()
        {
            XmlSerialization.WriteXmlFile(Contents, _clipboards_file);
        }

        private static async void Clipboard_ContentChanged(object sender, object e)
        {
            if (IsListenning)
            {
                var dataPackageView = Clipboard.GetContent();
                Log.Info("Formats: " + String.Join(",", dataPackageView.AvailableFormats));
                if (dataPackageView.Contains(StandardDataFormats.Text))
                {
                    var text = await dataPackageView.GetTextAsync();
                    //TODO
                    if (text.Length > 10000) { return; }
                    Contents.AddNewText(text);
                    Changed?.Invoke(text);
                }
            }
        }
    }
}
