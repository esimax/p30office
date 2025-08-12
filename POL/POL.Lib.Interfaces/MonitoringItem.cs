using System.ComponentModel;

namespace POL.Lib.Interfaces
{
    public class MonitoringItem : INotifyPropertyChanged
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

        #region Status

        private EnumMonitoringStatus _Status;

        public EnumMonitoringStatus Status
        {
            get { return _Status; }
            set
            {
                if (_Status == value) return;
                _Status = value;
                OnPropertyChanged("Status");
            }
        }

        #endregion

        #region Message

        private string _Message;

        public string Message
        {
            get { return _Message; }
            set
            {
                if (_Message == value) return;
                _Message = value;
                OnPropertyChanged("Message");
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
