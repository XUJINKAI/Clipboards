using DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using XJK;
using XJK.AOP;
using XJK.Serializers;
using XJK.SysX;
using XJK.WPF;

namespace WpfBackground
{
    public class Service : IService
    {
        public static Service Current { get; private set; }
        public static AppServiceInvoker AppServiceInvoker { get; private set; }
        public static IClient ClientProxy { get; private set; }

        private static Setting _setting = new Setting();
        public static Setting Setting
        {
            get => _setting;
            set
            {
                _setting = value;
                Clipboards.Contents.LimitToCapacity(value.ClipboardCapacity);
            }
        }

        private Service() { }

        public static void Init(string appservername, string packagefamilyname)
        {
            Current = new Service();
            LoadSetting();
            AppServiceInvoker = new AppServiceInvoker(packagefamilyname, appservername);
            Task.Run(async () => { await AppServiceInvoker.Connect(); });
            ClientProxy = MethodProxy.CreateProxy<IClient>(AppServiceInvoker);
            Log.ModuleId = ENV.ModuleHandleHex;
        }

        public static void LoadSetting()
        {
            if (File.Exists(App.SettingXmlFilePath))
            {
                try
                {
                    string xml = FS.ReadAllText(App.SettingXmlFilePath);
                    Setting = XmlSerialization.FromXmlText<Setting>(xml);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        public static void WriteSetting()
        {
            Setting.ToXmlText().WriteToAll(App.SettingXmlFilePath);
        }

        public void Dispose()
        {
            if (AppServiceInvoker != null)
            {
                AppServiceInvoker.DisposeConnection();
            }
        }

        #region interface

        public Task<bool> SetTopmost(bool topmost)
        {
            IntPtr handle = Handle.GetActiveWindow();
            Topmost.SetOrToggle(handle, topmost);
            var result = Topmost.Get(handle);
            return Task.FromResult(result);
        }

        public Task<bool> SetTopmostByTitle(string Title, bool topmost)
        {
            IntPtr hwnd = Handle.GetByTitle(Title);
            Topmost.SetOrToggle(hwnd, topmost);
            var result = Topmost.Get(hwnd);
            return Task.FromResult(result);
        }

        public Task ShowUwpWindow()
        {
            AppServiceInvoker.DispatchInvoke(() =>
            {
                WpfWindow.ShowWindow();
            });
            return Task.CompletedTask;
        }

        public Task Shutdown()
        {
            AppServiceInvoker.DispatchInvoke(() =>
            {
                WriteDataFile();
                WinMsg.DisposeHelperWindow();
                App.Current.Shutdown();
            });
            return Task.CompletedTask;
        }

        public Task WriteDataFile()
        {
            AppServiceInvoker.DispatchInvoke(() =>
            {
                Clipboards.WriteClipboardsFile();
                WriteSetting();
                Log.Debug("WriteDataFile");
            });
            return Task.CompletedTask;
        }


        public Task<Setting> GetSetting()
        {
            return Task.FromResult(Setting);
        }

        public Task<bool> SetSetting(Setting setting)
        {
            AppServiceInvoker.DispatchInvoke(() =>
            {
                Setting = setting;
            });
            return Task.FromResult(true);
        }


        public Task<ClipboardContents> GetClipboardContents()
        {
            return Task.FromResult(Clipboards.Contents);
        }

        public Task<bool> ClearClipboardList()
        {
            AppServiceInvoker.DispatchInvoke(() =>
            {
                Clipboards.Clear();
            });
            return Task.FromResult(true);
        }

        public Task SetClipboard(ClipboardItem clipboardItem)
        {
            AppServiceInvoker.DispatchInvoke(() =>
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
                    var randomStream = Converter.BytesToRandomAccessStream(imagebytes);
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
            });
            return Task.CompletedTask;
        }

        public Task SetClipboard(List<ClipboardItem> list)
        {
            AppServiceInvoker.DispatchInvoke(() =>
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
            });
            return Task.CompletedTask;
        }
        #endregion
    }
}
