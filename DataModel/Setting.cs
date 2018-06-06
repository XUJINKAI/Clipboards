using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    [Serializable]
    public class Setting
    {
        public int ClipboardCapacity { get; set; } = 10;
        public int ClipboardItemLimitSizeKb { get; set; } = 3000;
    }
}
