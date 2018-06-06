using DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel.DataTransfer;
using XJK;
using XJK.Serializers;
using XJK.SysX;

namespace WpfBackground
{
    static class Clipboards
    {
        private static string _clipboards_folder;
        private static string _clipboards_file;

        public static bool IsListenning { get; private set; } = false;

        private static ClipboardContents _clipboardContents;
        public static ClipboardContents Contents
        {
            get => _clipboardContents;
            set
            {
                if (_clipboardContents != null)
                {
                    _clipboardContents.CollectionChanged -= _clipboardContents_CollectionChanged;
                }
                _clipboardContents = value;
                if (_clipboardContents != null)
                {
                    _clipboardContents.CollectionChanged += _clipboardContents_CollectionChanged;
                }
            }
        }

        private static async void _clipboardContents_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
            await Service.ClientProxy.ClipboardCollectionChange(addItems, removeItems);
        }

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
            StartListen();
        }

        private static void LoadClipboardsFile()
        {
            if (File.Exists(_clipboards_file))
            {
                string xml = "";
                try
                {
                    xml = FS.ReadAllText(_clipboards_file);
                    var wrapper = XmlSerialization.FromXmlText<ClipboardWrapper>(xml);
                    Contents = wrapper.Contents;
                }
                catch (Exception ex)
                {
                    Log.Error("LoadClipboardsFile: Parse Clipboards XML Error", ex);
                }
            }
            if (Contents == null)
            {
                Contents = new ClipboardContents();
            }
        }

        public static void WriteClipboardsFile()
        {
            var wrapper = new ClipboardWrapper() { Contents = Contents };
            XmlSerialization.WriteXmlFile(wrapper, _clipboards_file);
        }

        private static async void Clipboard_ContentChanged(object sender, object e)
        {
            if (IsListenning)
            {
                var data = Clipboard.GetContent();
                Log.Info("Formats: " + string.Join(", ", data.AvailableFormats));
                ClipboardItem item = new ClipboardItem() { Type = ClipboardContentType.None };

                if (data.Contains(StandardDataFormats.Text))
                {
                    var text = await data.GetTextAsync();
                    if (System.Text.ASCIIEncoding.ASCII.GetByteCount(text) > Service.Setting.ClipboardItemLimitSizeKb * 1024)
                    {
                        return;
                    }
                    item.SetText(text);
                }
                if (data.Contains(StandardDataFormats.Bitmap))
                {
                    var streamReference = await data.GetBitmapAsync();
                    var randomAccessStreamWithContentType = await streamReference.OpenReadAsync();
                    if ((int)randomAccessStreamWithContentType.Size > Service.Setting.ClipboardItemLimitSizeKb * 1024)
                    {
                        return;
                    }
                    var bytes = Converter.RandomAccessStreamToBytes(randomAccessStreamWithContentType);
                    item.ImageBytes = bytes;
                }

                if (data.Contains(StandardDataFormats.Html))
                {
                    var x = await data.GetHtmlFormatAsync();
                    if (System.Text.ASCIIEncoding.ASCII.GetByteCount(x) > Service.Setting.ClipboardItemLimitSizeKb * 1024)
                    {
                        return;
                    }
                    item.Type = ClipboardContentType.Html;
                    item.StringContent = x;
                }
                else if(data.Contains(StandardDataFormats.Rtf))
                {
                    var x = await data.GetRtfAsync();
                    if (System.Text.ASCIIEncoding.ASCII.GetByteCount(x) > Service.Setting.ClipboardItemLimitSizeKb * 1024)
                    {
                        return;
                    }
                    item.Type = ClipboardContentType.Rtf;
                    item.StringContent = x;
                }
                if (item.Type != ClipboardContentType.None)
                {
                    Contents.AddNew(item, Service.Setting.ClipboardCapacity);
                }
            }
        }
    }
}
