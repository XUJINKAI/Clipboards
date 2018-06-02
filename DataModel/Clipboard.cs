using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataModel
{
    [Serializable]
    public class ClipboardItem
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string Text { get; set; }
        //TODO for image or other file
        public string FilePath { get; set; }

        public ClipboardItem()
        {
            Time = DateTime.Now;
        }

        public ClipboardItem(string text) : this()
        {
            Text = text;
        }
    }

    [Serializable]
    public class ClipboardContents
    {
        public int Capacity = 50;

        public int Ptr { get; set; } = 0;

        public List<ClipboardItem> List { get; set; }

        public ClipboardContents()
        {
            List = new List<ClipboardItem>();
        }

        private void Add(ClipboardItem item)
        {
            item.Id = Ptr++;
            if (List.Contains(item))
            {
                List.Remove(item);
            }
            while (List.Count > Capacity)
            {
                List.RemoveAt(0);
            }
            List.Add(item);
        }

        public void AddText(string text)
        {
            Add(new ClipboardItem(text));
        }

        public bool ContainsText(string text)
        {
            return List.Where(o => o.Text == text).Count() != 0;
        }

        public void Clear() => List.Clear();
    }
}
