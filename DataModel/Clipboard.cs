using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using XJK.Serializers;

namespace DataModel
{
    [Serializable]
    public class ClipboardWrapper
    {
        public ClipboardContents Contents { get; set; }
    }

    [Serializable]
    public class ClipboardContents : ObservableCollection<ClipboardItem>
    {
        public void LimitToCapacity(int capacity)
        {
            if (capacity > 0)
            {
                while (Count > capacity)
                {
                    RemoveAt(Count - 1);
                }
            }
        }

        public ClipboardContents()
        {

        }

        public void AddNew(ClipboardItem item, int capacity)
        {
            if (Contains(item))
            {
                Remove(item);
            }
            base.Insert(0, item);
            LimitToCapacity(capacity);
        }
    }
}
