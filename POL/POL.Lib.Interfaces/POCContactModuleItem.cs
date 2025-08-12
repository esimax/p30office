using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace POL.Lib.Interfaces
{
    public class POCContactModuleItem : INotifyPropertyChanged
    {
        private bool _IsChecked;
        public string Key { get; set; }
        public BitmapImage Image { get; set; }
        public string Title { get; set; }
        public string Tooltip { get; set; }
        public int Order { get; set; }
        public int Permission { get; set; }
        public ICommand Command { get; set; }

        public Type ViewType { get; set; }
        public Type ModelType { get; set; }

        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                if (_IsChecked == value) return;
                _IsChecked = value;
                OnPropertyChanged("IsChecked");
                if (value)
                    RaiseOnIsChecked();
            }
        }

        public bool InTamas { get; set; }


        public static Type LasteSelectedVmType { get; set; }


        public event EventHandler OnIsChecked;

        protected void RaiseOnIsChecked()
        {
            if (OnIsChecked == null) return;
            var temp = OnIsChecked;
            temp.Invoke(this, EventArgs.Empty);
        }

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
