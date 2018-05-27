using CommonLibrary;
using DataModel;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;

namespace WpfBackground
{
    static class Clipboards
    {
        public static event Action<string> Changed;
        public static bool IsListenning { get; private set; } = false;

        public static List<string> TextList;
        public static int TextCapacity { get; set; } = 20;
        
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
            TextList.Clear();
        }

        static Clipboards()
        {
            TextList = new List<string>();
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
                    if (TextList.Contains(text))
                    {
                        TextList.Remove(text);
                    }
                    while (TextList.Count > TextCapacity)
                    {
                        TextList.RemoveAt(0);
                    }
                    TextList.Add(text);
                    Changed?.Invoke(text);
                }
            }
        }
    }
}
