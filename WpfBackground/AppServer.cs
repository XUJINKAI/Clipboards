using CommonLibrary;
using System;
using WpfBackground;

namespace DataModel
{
    public static class AppServer
    {
        public static void Init()
        {
            AppServiceConnect.Received += Recive;
            AppServiceConnect.TryConnect();
            Clipboards.Changed += Clipboards_Changed;
            Clipboards.StartListen();
        }

        private static void Clipboards_Changed(string text)
        {
            SendNewClipboardItem(text);
        }

        public static void Recive(ConnectionData data)
        {
            Log.Verbose($"AppServer.Recive: {data}");
            if (data.Direction == Direction.AskServer)
            {
                switch (data.Command)
                {
                    case Command.SetTopmost:
                        WinTopmost.ToggleTopmost(null, true);
                        break;
                    case Command.ShowWindow:
                        WpfWindow.ShowWindow();
                        break;
                    case Command.ShutDown:
                        App.ShutDown();
                        break;
                    case Command.ClipboardList:
                        SendClipboardList();
                        break;
                }
            }
            else if (data.Direction == Direction.UpdateServer)
            {
                switch (data.Command)
                {
                    case Command.IsOpen:

                        break;
                    case Command.ClearClipboardList:
                        Clipboards.Clear();
                        break;
                    case Command.SetTopmost:
                        WinTopmost.ToggleTopmost(null, true);
                        break;
                }
            }
        }

        public static void SendNewClipboardItem(string text)
        {
            AppServiceConnect.Send(new ConnectionData(Direction.UpdateUi, Command.AddClipboardItem) { Text = text });
        }

        public static void SendClipboardList()
        {
            AppServiceConnect.Send(new ConnectionData(Direction.UpdateUi, Command.ClipboardList, new ExChangeData() { ClipboardList = Clipboards.TextList }));
        }
    }
}
