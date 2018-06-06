using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public interface IClient
    {
        Task ClipboardCollectionChange(List<ClipboardItem> addItems, List<ClipboardItem> removeItems);
    }

    public interface IService
    {
        Task<bool> SetTopmost(bool topmost);
        Task<bool> SetTopmostByTitle(string Title, bool topmost);
        Task ShowUwpWindow();
        Task Shutdown();
        Task WriteDataFile();

        Task<Setting> GetSetting();
        Task<bool> SetSetting(Setting setting);

        Task SetClipboard(ClipboardItem clipboardItem);
        Task SetClipboard(List<ClipboardItem> list);
        Task<ClipboardContents> GetClipboardContents();
        Task<bool> ClearClipboardList();
    }
}
