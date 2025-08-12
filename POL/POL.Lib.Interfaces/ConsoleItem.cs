using System;
using System.ComponentModel;
using Microsoft.Practices.Prism.Logging;

namespace POL.Lib.Interfaces
{
    public class ConsoleItem : INotifyPropertyChanged
    {
        #region ConsoleDate

        private DateTime _ConsoleDate;

        public DateTime ConsoleDate
        {
            get { return _ConsoleDate; }
            set
            {
                if (_ConsoleDate == value) return;
                _ConsoleDate = value;
                OnPropertyChanged("ConsoleDate");
            }
        }

        #endregion

        #region ConsoleType

        private Category _ConsoleType;

        public Category ConsoleType
        {
            get { return _ConsoleType; }
            set
            {
                if (_ConsoleType == value) return;
                _ConsoleType = value;
                OnPropertyChanged("ConsoleType");
            }
        }

        #endregion

        #region ConsoleText

        private string _ConsoleText;

        public string ConsoleText
        {
            get { return _ConsoleText; }
            set
            {
                if (_ConsoleText == value) return;
                _ConsoleText = value;
                OnPropertyChanged("ConsoleText");
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
