using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
    public enum MsgType
    {
        None,
        Query,
        Set,
        Confirm,
    }

    public enum Command
    {
        None,
        IsOpen,
        ShowWindow,
        ShutDown,
        Topmost,
        ClipboardList,
        AddClipboardItem,
        ClearClipboardList,
    }

    public class ExChangeData
    {
        public List<string> ClipboardList { get; set; }
    }
}
