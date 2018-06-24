using DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfService.Model;
using XJK;
using XJK.Serializers;
using XJK.SysX;

namespace WpfService
{
    public class Controller : IService
    {
        public static IClient ClientProxy { get; private set; }


        public static Controller Instance { get; private set; }

        static Controller()
        {
            Instance = new Controller();
        }

        private Controller() { }

        #region IService
        public Task<bool> ClearClipboardList()
        {
            throw new NotImplementedException();
        }

        public Task<ClipboardWrapper> GetClipboardWrapper()
        {
            throw new NotImplementedException();
        }

        public Task<Setting> GetSetting()
        {
            throw new NotImplementedException();
        }

        public Task SetClipboard(ClipboardItem clipboardItem)
        {
            throw new NotImplementedException();
        }

        public Task SetClipboard(List<ClipboardItem> list)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetSetting(Setting setting)
        {
            throw new NotImplementedException();
        }

        public Task ShowUwpWindow()
        {
            throw new NotImplementedException();
        }

        public Task Shutdown()
        {
            throw new NotImplementedException();
        }

        public Task WriteDataFile()
        {
            throw new NotImplementedException();
        }
        #endregion

        public static readonly string ApplicationDataSpecialFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static readonly string AppDataFolder = FS.CreateFolder(Path.Combine(ApplicationDataSpecialFolder, "Clipboards"));
        public static readonly string SettingXmlFilePath = Path.Combine(AppDataFolder, "Setting.xml");
        public static readonly string ClipboardXmlFilePath = Path.Combine(AppDataFolder, "Clipboards.xml");
        public static readonly string ClipboardContentFolder = FS.CreateFolder(Path.Combine(AppDataFolder, "ClipboardContents"));

        public static void ReadClipboardsFile()
        {
            if (File.Exists(ClipboardXmlFilePath))
            {
                string xml = "";
                try
                {
                    xml = FS.ReadAllText(ClipboardXmlFilePath);
                    var wrapper = XmlSerialization.FromXmlText<ClipboardWrapper>(xml);
                    Clipboards.Instance.ClipboardWrapper = wrapper;
                }
                catch (Exception ex)
                {
                    Log.Write("Parse Clipboards XML Error");
                    Log.Error(ex);
                }
            }
        }

        public static void WriteClipboardsFile()
        {
            Clipboards.Instance.ClipboardWrapper.ToXmlText().WriteToAll(ClipboardXmlFilePath);
        }

        public static void ReadSettingFile()
        {
            if (File.Exists(SettingXmlFilePath))
            {
                string xml = "";
                try
                {
                    xml = FS.ReadAllText(SettingXmlFilePath);
                    var setting = XmlSerialization.FromXmlText<Setting>(xml);
                }
                catch (Exception ex)
                {
                    Log.Write("Parse Clipboards XML Error");
                    Log.Error(ex);
                }
            }
        }

        public static void WriteSettingFile()
        {
            Setting setting = new Setting()
            {

            };
            setting.ToXmlText().WriteToAll(SettingXmlFilePath);
        }
    }
}
