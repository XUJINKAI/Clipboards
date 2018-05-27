using CommonLibrary;
using DataModel;

namespace WpfBackground
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
            AppServiceConnect.Send(new ConnectionData(MsgType.Set, Command.AddClipboardItem) { Text = text });
        }

        public static void Recive(ConnectionData data)
        {
            Log.Verbose($"AppServer.Recive: {data}");
            switch (data.Command)
            {
                case Command.ShowWindow:
                    WpfWindow.ShowWindow();
                    break;
                case Command.ShutDown:
                    App.ShutDown();
                    break;
                default:
                    if (data.Type == MsgType.Query)
                    {
                        switch (data.Command)
                        {
                            case Command.Topmost:
                                AppServiceConnect.Send(new ConnectionData(MsgType.Set, Command.Topmost) { Boolean = Topmost.Get() });
                                break;
                            case Command.ClipboardList:
                                AppServiceConnect.Send(new ConnectionData(MsgType.Set, Command.ClipboardList, new ExChangeData() { ClipboardList = Clipboards.TextList }));
                                break;
                        }
                    }
                    else if (data.Type == MsgType.Set)
                    {
                        switch (data.Command)
                        {
                            case Command.Topmost:
                                Topmost.SetOrToggle(null, true);
                                break;
                            case Command.ClearClipboardList:
                                Clipboards.Clear();
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
