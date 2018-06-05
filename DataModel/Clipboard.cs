using CommonLibrary;
using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace DataModel
{
    [Serializable]
    public enum ClipboardContentType
    {
        Text,
        Image,
    }

    [Serializable]
    public class ClipboardItem
    {
        public DateTime Time { get; set; }
        public ClipboardContentType Type { get; set; }
        public string Base64 { get; set; }

        [XmlIgnore]
        public string Text
        {
            get
            {
                if (Type == ClipboardContentType.Text)
                {
                    return BinarySerialization.FromBase64BinaryString<string>(Base64);
                }
                else
                {
                    return $"<{Type}>";
                }
            }
            set
            {
                Type = ClipboardContentType.Text;
                Base64 = value.ToBase64BinaryString();
            }
        }
        
        public ClipboardItem()
        {
            Time = DateTime.Now;
        }

        public ClipboardItem(string text) : this()
        {
            Text = text;
        }

        public override bool Equals(object obj)
        {
            return obj is ClipboardItem item && item.Type == Type && item.Base64 == Base64;
        }

        public override int GetHashCode()
        {
            return $"[ClipboardItem]{Type}:{Base64}".GetHashCode();
        }
    }

    [Serializable]
    public class ClipboardContents : ObservableCollection<ClipboardItem>
    {
        private int _capacity;

        public int Capacity
        {
            get => _capacity; set
            {
                _capacity = value;
                while (Count > _capacity)
                {
                    RemoveAt(0);
                }
            }
        }

        public ClipboardContents()
        {
            Capacity = 50;
        }

        public void AddNew(ClipboardItem item)
        {
            if (Contains(item))
            {
                Remove(item);
            }
            base.Insert(0, item);
        }
    }
}
