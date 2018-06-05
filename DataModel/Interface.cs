using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public interface IClient
    {
        Task AddClipboardItem(ClipboardItem clipboardItem);
    }

    public interface IService
    {
        Task<bool> SetTopmost(bool topmost);
        Task<bool> SetTopmostByTitle(string Title, bool topmost);
        Task ShowUwpWindow();
        Task Shutdown();
        Task SetClipboard(ClipboardItem clipboardItem);
        Task<ClipboardContents> GetClipboardContents();
        Task<bool> ClearClipboardList();
        Task<Setting> GetSetting();
    }
}
