using DataModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using XJK;
using static WpfService.Model.RandomAccessStreamConverter;

namespace WpfService.Model
{
    public class Clipboards : INotifyPropertyChanged
    {
        public static Clipboards Instance { get; private set; }
        private Clipboards() { }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }


        private bool _isListenning = false;
        private ClipboardWrapper _clipboardWrapper;

        public bool IsListenning
        {
            get => _isListenning;
            set
            {
                if (_isListenning != value)
                {
                    if (value)
                    {
                        Clipboard.ContentChanged += SystemClipboard_ContentChanged;
                    }
                    else
                    {
                        Clipboard.ContentChanged -= SystemClipboard_ContentChanged;
                    }
                    _isListenning = value;
                    OnPropertyChanged("IsListenning");
                }
            }
        }

        public ClipboardWrapper ClipboardWrapper
        {
            get => _clipboardWrapper; set
            {
                if (_clipboardWrapper != null)
                {
                    _clipboardWrapper.PropertyChanged -= _clipboardWrapper_PropertyChanged;
                    _clipboardWrapper.CollectionChanged -= _clipboardWrapper_CollectionChanged;
                }
                _clipboardWrapper = value;
                if (_clipboardWrapper != null)
                {
                    _clipboardWrapper.PropertyChanged += _clipboardWrapper_PropertyChanged;
                    _clipboardWrapper.CollectionChanged += _clipboardWrapper_CollectionChanged;
                }
                OnPropertyChanged("ClipboardWrapper");
            }
        }

        private void _clipboardWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("ClipboardWrapper");
        }

        private void _clipboardWrapper_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<ClipboardItem> addItems = new List<ClipboardItem>();
            List<ClipboardItem> removeItems = new List<ClipboardItem>();
            if (e.NewItems != null)
            {
                foreach (var x in e.NewItems)
                {
                    addItems.Add((ClipboardItem)x);
                }
            }
            if (e.OldItems != null)
            {
                foreach (var x in e.OldItems)
                {
                    removeItems.Add((ClipboardItem)x);
                }
            }
            ClipboardChanged?.Invoke(addItems, removeItems);
            OnPropertyChanged("ClipboardWrapper");
        }

        static Clipboards()
        {
            Instance = new Clipboards
            {
                ClipboardWrapper = new ClipboardWrapper()
            };
        }

        //

        public static event Action<List<ClipboardItem>, List<ClipboardItem>> ClipboardChanged;

        public static void Clear()
        {
            Instance.ClipboardWrapper.Clear();
        }

        internal static void RemoveItems(List<ClipboardItem> ClipboardItems)
        {
            foreach (var item in ClipboardItems)
            {
                Instance.ClipboardWrapper.Remove(item);
            }
        }

        private static async void SystemClipboard_ContentChanged(object sender, object e)
        {
            if (Instance.IsListenning)
            {
                var data = Clipboard.GetContent();
                Log.Info("Formats: " + string.Join(", ", data.AvailableFormats));
                ClipboardItem item = new ClipboardItem() { Type = ClipboardContentType.None };

                if (data.Contains(StandardDataFormats.Text))
                {
                    var text = await data.GetTextAsync();
                    item.SetText(text);
                }
                if (data.Contains(StandardDataFormats.Bitmap))
                {
                    var streamReference = await data.GetBitmapAsync();
                    var randomAccessStreamWithContentType = await streamReference.OpenReadAsync();
                    var bytes = RandomAccessStreamToBytes(randomAccessStreamWithContentType);
                    item.ImageBytes = bytes;
                }

                if (data.Contains(StandardDataFormats.Html))
                {
                    var x = await data.GetHtmlFormatAsync();
                    item.Type = ClipboardContentType.Html;
                    item.StringContent = x;
                }
                else if (data.Contains(StandardDataFormats.Rtf))
                {
                    var x = await data.GetRtfAsync();
                    item.Type = ClipboardContentType.Rtf;
                    item.StringContent = x;
                }
                if (item.Type != ClipboardContentType.None)
                {
                    Instance.ClipboardWrapper.Add(item);
                }
            }
        }

        public static void SetClipboard(ClipboardItem clipboardItem)
        {
            DataPackage dataPackage = new DataPackage();
            string text = clipboardItem.PureText;
            if (!string.IsNullOrEmpty(text))
            {
                dataPackage.SetText(text);
            }
            var imagebytes = clipboardItem.ImageBytes;
            if (imagebytes != null && imagebytes.Length != 0)
            {
                var randomStream = BytesToRandomAccessStream(imagebytes);
                dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(randomStream));
            }
            switch (clipboardItem.Type)
            {
                case ClipboardContentType.Html:
                    dataPackage.SetHtmlFormat(clipboardItem.StringContent);
                    break;
                case ClipboardContentType.Rtf:
                    dataPackage.SetRtf(clipboardItem.StringContent);
                    break;
            }
            Clipboard.SetContent(dataPackage);
        }

        public static void SetClipboard(List<ClipboardItem> list)
        {
            if (list.Count == 1)
            {
                SetClipboard(list[0]);
            }
            else if (list.Count > 1)
            {
                DataPackage dataPackage = new DataPackage();
                var xml = new XmlDocument();
                var body = xml.CreateElement("div");
                xml.AppendChild(body);
                string htmlpattern = @"\<body\>(.*)\<\/body\>";
                Regex htmlreg = new Regex(htmlpattern, RegexOptions.IgnoreCase);
                foreach (var x in list)
                {
                    string text = x.PureText;
                    if (!string.IsNullOrEmpty(text))
                    {
                        var p = xml.CreateElement("p");
                        p.InnerText = text;
                        body.AppendChild(p);
                    }
                }
                dataPackage.SetText(xml.InnerText);
                //dataPackage.SetHtmlFormat(HtmlFormatHelper.CreateHtmlFormat(xml.InnerXml));
                Clipboard.SetContent(dataPackage);
            }
        }
    }
}
