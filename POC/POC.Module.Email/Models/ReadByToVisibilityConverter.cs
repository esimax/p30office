using System;
using System.Windows;
using System.Windows.Data;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;

namespace POC.Module.Email.Models
{
    public class ReadByToVisibilityConverter : IValueConverter
    {
        private IMembership _AMemebership;
        private IMembership AMemebership
        {
            get { return _AMemebership ?? (_AMemebership = ServiceLocator.Current.GetInstance<IMembership>()); }
        }

        private string _Username;
        private string UserName
        {
            get
            {
                if (AMemebership == null || AMemebership.ActiveUser == null)
                    return _Username;
                return _Username ?? (_Username = "|" + AMemebership.ActiveUser.UserName + "|");
            }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value == null) return Visibility.Collapsed;
                var s = value.ToString();
                return s.Contains(UserName) ? Visibility.Visible : Visibility.Collapsed;
            }
            catch
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
