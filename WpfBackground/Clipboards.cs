using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBackground
{
    static class Clipboards
    {
        public static bool Listenning { get; private set; } = false;

        public static List<string> TextList;
        public static int TextCapacity { get; set; } = 100;

        public static void StartListen()
        {
            if (Listenning == false)
            {
                Windows.ApplicationModel.DataTransfer.Clipboard.ContentChanged += Clipboard_ContentChanged;
            }
            Listenning = true;
        }

        public static void StopListen()
        {
            if(Listenning== true)
            {
                Windows.ApplicationModel.DataTransfer.Clipboard.ContentChanged -= Clipboard_ContentChanged;
            }
            Listenning = false;
        }

        static Clipboards()
        {
            TextList = new List<string>();
        }

        private static async void Clipboard_ContentChanged(object sender, object e)
        {
            if (Listenning)
            {
                var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
                if (dataPackageView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Text))
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
                }
            }
        }
    }
}
