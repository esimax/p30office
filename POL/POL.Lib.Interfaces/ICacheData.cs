using System.Collections.ObjectModel;
using System.ComponentModel;

namespace POL.Lib.Interfaces
{
    public interface ICacheData
    {
        void RaiseCacheChanged(EnumCachDataType type);
        void ForcePopulateCache(EnumCachDataType type, bool rebuild, object db);
        void PopulateAll();


        ObservableCollection<CacheItemProfileItem> GetProfileItemList();
        void SetProfileItemList(ObservableCollection<CacheItemProfileItem> list);

        ObservableCollection<CacheItemRoleItem> GetRoleList();
        void SetRoleList(ObservableCollection<CacheItemRoleItem> list);

        ObservableCollection<CacheItemCountry> GetCountryList();
        void SetRoleList(ObservableCollection<CacheItemCountry> list);

        ObservableCollection<CacheItemContactCat> GetContactCatList();
        void SetRoleList(ObservableCollection<CacheItemContactCat> list);

        ObservableCollection<CacheItemProfileTable> GetProfileTableList();
        void SetProfileTableList(ObservableCollection<CacheItemProfileTable> list);
    }

    public class CacheItemRoleItem : INotifyPropertyChanged
    {
        public object Tag { get; set; }

        #region Title

        private string _Title;

        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;
                _Title = value;
                OnPropertyChanged("Title");
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class CacheItemContactCat : INotifyPropertyChanged
    {
        public object Tag { get; set; }

        #region Title

        private string _Title;

        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;
                _Title = value;
                OnPropertyChanged("Title");
            }
        }

        #endregion

        #region Role

        private string _Role;

        public string Role
        {
            get { return _Role; }
            set
            {
                if (_Role == value) return;
                _Role = value;
                OnPropertyChanged("Role");
            }
        }

        #endregion

        #region ProfileRoots

        private string _ProfileRoots;

        public string ProfileRoots
        {
            get { return _ProfileRoots; }
            set
            {
                if (_ProfileRoots == value) return;
                _ProfileRoots = value;
                OnPropertyChanged("ProfileRoots");
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class CacheItemCountry : INotifyPropertyChanged
    {
        public object Tag { get; set; }

        #region Title

        private string _Title;

        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;
                _Title = value;
                OnPropertyChanged("Title");
            }
        }

        #endregion

        #region ISO3

        private string _ISO3;

        public string ISO3
        {
            get { return _ISO3; }
            set
            {
                if (_ISO3 == value) return;
                _ISO3 = value;
                OnPropertyChanged("ISO3");
            }
        }

        #endregion

        #region TeleCodeString

        private string _TeleCodeString;

        public string TeleCodeString
        {
            get { return _TeleCodeString; }
            set
            {
                if (_TeleCodeString == value) return;
                _TeleCodeString = value;
                OnPropertyChanged("TeleCodeString");
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class CacheItemProfileTable : INotifyPropertyChanged
    {
        public CacheItemProfileTable()
        {
            ChildList = new ObservableCollection<CacheItemProfileTValue>();
        }

        public object Tag { get; set; }

        public override string ToString()
        {
            return Title;
        }

        #region Title

        private string _Title;

        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;
                _Title = value;
                OnPropertyChanged("Title");
            }
        }

        #endregion

        #region ValueDepth

        private int _ValueDepth;

        public int ValueDepth
        {
            get { return _ValueDepth; }
            set
            {
                if (_ValueDepth == value) return;
                _ValueDepth = value;
                OnPropertyChanged("ValueDepth");
            }
        }

        #endregion

        #region ChildList

        private ObservableCollection<CacheItemProfileTValue> _ChildList;

        public ObservableCollection<CacheItemProfileTValue> ChildList
        {
            get { return _ChildList; }
            set
            {
                _ChildList = value;
                OnPropertyChanged("ChildList");
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class CacheItemProfileTValue : INotifyPropertyChanged
    {
        public CacheItemProfileTValue()
        {
            ChildList = new ObservableCollection<CacheItemProfileTValue>();
        }

        public object Tag { get; set; }

        public override string ToString()
        {
            return Title;
        }

        #region Title

        private string _Title;

        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;
                _Title = value;
                OnPropertyChanged("Title");
            }
        }

        #endregion

        #region Order

        private int _Order;

        public int Order
        {
            get { return _Order; }
            set
            {
                if (_Order == value) return;
                _Order = value;
                OnPropertyChanged("Order");
            }
        }

        #endregion

        #region ChildList

        private ObservableCollection<CacheItemProfileTValue> _ChildList;

        public ObservableCollection<CacheItemProfileTValue> ChildList
        {
            get { return _ChildList; }
            set
            {
                _ChildList = value;
                OnPropertyChanged("ChildList");
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
