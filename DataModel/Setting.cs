﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    [Serializable]
    public class Setting
    {
        public int ClipboardsCapacity { get; set; } = 50;
    }
}
