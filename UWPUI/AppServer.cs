using CommonLibrary;
using System;
using UWPUI;
using Windows.UI.Xaml;

namespace DataModel
{
    public static class AppServer
    {
        public static void Recive(ConnectionData data)
        {
            Log.Verbose(data.ToString());
            if (data.Direction == Direction.AskUi)
            {
                switch (data.Command)
                {
                    case Command.IsOpen:
                        App.Send(new ConnectionData(Direction.UpdateServer, Command.IsOpen) { Boolean = true });
                        break;
                }
            }
            else if (data.Direction == Direction.UpdateUi)
            {
                if(data.Command == Command.AddClipboardItem)
                {
                    MainPage.Instance.AddClipboardList(data.Text);
                }
                if (data.Command == Command.ClipboardList)
                {
                    MainPage.Instance.SetClipboardList(data.Data.ClipboardList);
                }
            }
        }

        public static void RequestLists()
        {
            Log.Verbose("Request Clipboard Lists");
            App.Send(new ConnectionData(Direction.AskServer, Command.ClipboardList));
        }


        public static void RequestTopmost()
        {
            App.Send(new ConnectionData(Direction.AskServer, Command.SetTopmost));
        }

    }
}
