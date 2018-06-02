using CommonLibrary;
using DataModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace UWPUI
{
    public class Client : IClient
    {
        public static Client Instance = new Client();

        public ClipboardContents ClipboardContents = new ClipboardContents();

        public string LogText { get; private set; } = "";
        public event Action<string> LogChanged;

        private Client()
        {
            Log.AutoStringListener += Log_AutoStringListener;
            Log.AutoFlush = true;
        }

        public async Task RequestClipboardContentsAsync()
        {
            ClipboardContents = (ClipboardContents)await AppServiceReciver.Invoke(s => s.GetClipboardContents());
        }

        private void Log_AutoStringListener(string obj)
        {
            LogText += obj;
            LogChanged?.Invoke(LogText);
        }

        public void AddClipboardItem(ClipboardItem clipboardItem)
        {
            ClipboardContents.List.Insert(0, clipboardItem);
        }
    }
}
