using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
    public interface IClient
    {
        void AddClipboardItem(ClipboardItem clipboardItem);
    }

    public interface IService
    {
        bool SetTopmost(bool topmost);
        void ShowUwpWindow();
        void Shutdown();
        bool SetClipboard(ClipboardItem clipboardItem);
        ClipboardContents GetClipboardContents();
        bool ClearClipboardList();
        Setting GetSetting();
    }
}
