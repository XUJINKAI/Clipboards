using CommonLibrary;
using DataModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;

namespace UWPUI
{
    public class ClipboardItem
    {
        public string Text { get; set; } = "";

        public ClipboardItem() { }

        public ClipboardItem(string text)
        {
            Text = text;
        }
    }

    public static class AppServer
    {
        public static readonly ObservableCollection<ClipboardItem> ClipboardContents = new ObservableCollection<ClipboardItem>();

        public static string LogText { get; private set; } = "";
        public static event Action<string> LogChanged;

        public static void Init()
        {
            Log.AutoStringListener += Log_AutoStringListener;
            Log.AutoFlush = true;
        }

        private static void Log_AutoStringListener(string obj)
        {
            LogText += obj;
            LogChanged?.Invoke(LogText);
        }

        public static void Recive(ConnectionData data)
        {
            Log.Verbose(data.ToString());
            if (data.Type == MsgType.Query)
            {
                switch (data.Command)
                {
                    case Command.IsOpen:
                        break;
                }
            }
            else if (data.Type == MsgType.Set)
            {
                if(data.Command == Command.AddClipboardItem)
                {
                    if (ClipboardContents.Where(o => o.Text == data.Text).Count() == 0)
                    {
                        ClipboardContents.Insert(0, new ClipboardItem(data.Text));
                    }
                }
                if (data.Command == Command.ClipboardList)
                {
                    data.Data.ClipboardList.Reverse();
                    ClipboardContents.Clear();
                    foreach(var x in data.Data.ClipboardList)
                    {
                        ClipboardContents.Add(new ClipboardItem(x));
                    }
                }
            }
        }

        public static void RequestGetClipboardList()
        {
            Log.Verbose("Request Clipboard Lists");
            App.Send(new ConnectionData(MsgType.Query, Command.ClipboardList));
        }

        public static void RequestClearClipboardList()
        {
            Log.Verbose("Request Clear Clipboard Lists");
            App.Send(new ConnectionData(MsgType.Set, Command.ClearClipboardList));
            ClipboardContents.Clear();
        }

        public static void RequestSetTopmost(bool top)
        {
            Log.Verbose("Request Topmost");
            App.Send(new ConnectionData(MsgType.Set, Command.Topmost) { Boolean = top });
        }

        public static void SetClipboard(string Text)
        {
            var data = new DataPackage();
            data.SetText(Text);
            Clipboard.SetContent(data);
        }

    }
}
