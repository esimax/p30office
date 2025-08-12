using System.Collections.ObjectModel;
using System.ComponentModel;

namespace POL.Lib.Interfaces
{
    public class CacheItemProfileItem : INotifyPropertyChanged
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

        #region ImageUriString

        private string _ImageUriString;

        public string ImageUriString
        {
            get { return _ImageUriString; }
            set
            {
                if (_ImageUriString == value) return;
                _ImageUriString = value;
                OnPropertyChanged("ImageUriString");
            }
        }

        #endregion

        #region ProfileItemType

        private EnumProfileItemType _ProfileItemType;

        public EnumProfileItemType ProfileItemType
        {
            get { return _ProfileItemType; }
            set
            {
                if (_ProfileItemType == value) return;
                _ProfileItemType = value;
                OnPropertyChanged("ProfileItemType");
            }
        }

        #endregion

        #region TreeLevel

        private int _TreeLevel;

        public int TreeLevel
        {
            get { return _TreeLevel; }
            set
            {
                if (_TreeLevel == value) return;
                _TreeLevel = value;
                OnPropertyChanged("TreeLevel");
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

        #region InSearch

        private bool _InSearch;

        public bool InSearch
        {
            get { return _InSearch; }
            set
            {
                if (_InSearch == value) return;
                _InSearch = value;
                OnPropertyChanged("InSearch");
            }
        }

        #endregion

        #region RoleViewer

        private string _RoleViewer;

        public string RoleViewer
        {
            get { return _RoleViewer; }
            set
            {
                if (_RoleViewer == value) return;
                _RoleViewer = value;
                OnPropertyChanged("RoleViewer");
            }
        }

        #endregion

        #region RoleEditor

        private string _RoleEditor;

        public string RoleEditor
        {
            get { return _RoleEditor; }
            set
            {
                if (_RoleEditor == value) return;
                _RoleEditor = value;
                OnPropertyChanged("RoleEditor");
            }
        }

        #endregion

        #region CategoriesString

        private string _CategoriesString;

        public string CategoriesString
        {
            get { return _CategoriesString; }
            set
            {
                if (_CategoriesString == value) return;
                _CategoriesString = value;
                OnPropertyChanged("CategoriesString");
            }
        }

        #endregion

        #region ChildList

        private ObservableCollection<CacheItemProfileItem> _ChildList;

        public ObservableCollection<CacheItemProfileItem> ChildList
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
