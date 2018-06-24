using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using XJK;

namespace DataModel
{
    [Serializable]
    public class ClipboardWrapper : INotifyPropertyChanged
    {
        private ObservableCollection<ClipboardItem> _contents;
        private ClipboardSetting _setting;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        public ClipboardSetting Setting
        {
            get => _setting; set
            {
                if (_setting != null)
                {
                    _setting.PropertyChanged -= _setting_PropertyChanged;
                }
                _setting = value;
                if (_setting != null)
                {
                    _setting.PropertyChanged += _setting_PropertyChanged;
                }
                OnPropertyChanged("Setting");
            }
        }

        private void _setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            LimitToCapacity();
            LimitToSize();
            OnPropertyChanged("Setting");
        }

        public int Count() => Contents.Count();
        public void Clear() => Contents.Clear();
        public bool Remove(ClipboardItem item) => Contents.Remove(item);
        public bool Contains(ClipboardItem item) => Contents.Contains(item);

        public delegate void CollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e);
        public event CollectionChangedHandler CollectionChanged;

        public ObservableCollection<ClipboardItem> Contents
        {
            get => _contents; set
            {
                if (_contents != null)
                {
                    _contents.CollectionChanged -= _contents_CollectionChanged;
                }
                _contents = value;
                if (_contents != null)
                {
                    _contents.CollectionChanged += _contents_CollectionChanged;
                }
                OnPropertyChanged("Contents");
            }
        }

        private void _contents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(sender, e);
            OnPropertyChanged("Contents");
        }

        public ClipboardWrapper()
        {
            Setting = new ClipboardSetting();
            Contents = new ObservableCollection<ClipboardItem>();
        }

        public void LimitToCapacity()
        {
            if (Setting.Capacity > 0)
            {
                while (Contents.Count > Setting.Capacity)
                {
                    Contents.RemoveAt(Contents.Count - 1);
                }
            }
        }

        public void LimitToSize()
        {
            if (Setting.ItemLimitSizeKb > 0)
            {
                var list = Contents.Where(item => item.MaxSizeBytes > Setting.ItemLimitSizeKb);
                foreach(var item in list)
                {
                    Contents.Remove(item);
                }
            }
        }

        public void Add(ClipboardItem item)
        {
            if (item.MaxSizeBytes > Setting.ItemLimitSizeKb * 1024)
            {
                Log.Info($"Item too big: {item.MaxSizeBytes}");
                return;
            }
            if (Contents.Contains(item))
            {
                Contents.Remove(item);
            }
            Contents.Insert(0, item);
            LimitToCapacity();
        }
    }

    [Serializable]
    public class ClipboardSetting : INotifyPropertyChanged
    {
        private int _capacity = 10;
        private int _itemLimitSizeKb = 5000;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        public int Capacity
        {
            get => _capacity; set
            {
                _capacity = value;
                OnPropertyChanged("Capacity");
            }
        }
        public int ItemLimitSizeKb
        {
            get => _itemLimitSizeKb; set
            {
                _itemLimitSizeKb = value;
                OnPropertyChanged("ItemLimitSizeKb");
            }
        }
    }
}
