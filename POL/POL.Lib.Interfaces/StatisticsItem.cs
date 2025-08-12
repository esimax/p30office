using System.ComponentModel;

namespace POL.Lib.Interfaces
{
    public class StatisticsItem : INotifyPropertyChanged
    {
        public string Key { get; set; }

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

        #region Value

        private string _Value;

        public string Value
        {
            get { return _Value; }
            set
            {
                if (_Value == value) return;
                _Value = value;
                OnPropertyChanged("Value");
            }
        }

        #endregion

        #region Tag

        private object _Tag;

        public object Tag
        {
            get { return _Tag; }
            set
            {
                if (_Tag == value) return;
                _Tag = value;
                OnPropertyChanged("Tag");
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
