using System;
using System.Windows;
using System.Windows.Data;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;

namespace POC.Module.Email.Models
{
    public class ReadByToVisibilityInverseConverter : IValueConverter
    {
        private IMembership _AMemebership;
        private IMembership AMemebership
        {
            get
            {
                return _AMemebership ?? (_AMemebership = ServiceLocator.Current.GetInstance<IMembership>());
            }
        }

        private string _Username;
        private string UserName
        {
            get { return _Username ?? (_Username = "|" + AMemebership.ActiveUser.UserName + "|"); }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value == null) return Visibility.Visible;
                var s = value.ToString();
                return s.Contains(UserName) ? Visibility.Collapsed : Visibility.Visible;
            }
            catch
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
