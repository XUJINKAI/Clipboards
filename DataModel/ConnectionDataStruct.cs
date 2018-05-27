using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
    public enum Direction
    {
        None,
        AskServer,
        UpdateServer,
        AskUi,
        UpdateUi,
    }

    public enum Command
    {
        None,
        IsOpen,
        ShowWindow,
        ShutDown,
        SetTopmost,
        ClipboardList,
        AddClipboardItem,
        ClearClipboardList,
    }

    public class ExChangeData
    {
        public List<string> ClipboardList { get; set; }
    }
}
