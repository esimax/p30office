using System;
using System.ComponentModel;

namespace POL.WPF.DXControls.MVVM
{
    public class NotifyObjectBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void RaisePropertyChanged(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new ArgumentException(string.Format("{0} doesn't exist in {1} type", propertyName, GetType().Name));
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
