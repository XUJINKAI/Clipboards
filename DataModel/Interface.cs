using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataModel
{
    public interface IClient
    {
        Task ClipboardCollectionChange(List<ClipboardItem> addItems, List<ClipboardItem> removeItems);
    }

    public interface IService
    {
        Task ShowUwpWindow();
        Task Shutdown();
        Task WriteDataFile();

        Task<Setting> GetSetting();
        Task<bool> SetSetting(Setting setting);

        Task SetClipboard(ClipboardItem clipboardItem);
        Task SetClipboard(List<ClipboardItem> list);
        Task<ClipboardWrapper> GetClipboardWrapper();
        Task<bool> ClearClipboardList();
    }
}
