using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;

namespace POL.Lib.Common
{
    public class ConvCardTableUserCreated : IValueConverter
    {
        private string _CurrentUser;

        private string CurrentUser
        {
            get
            {
                if (string.IsNullOrEmpty(_CurrentUser))
                {
                    try
                    {
                        var AMembership = ServiceLocator.Current.GetInstance<IMembership>();
                        if (AMembership == null) return string.Empty;
                        _CurrentUser = AMembership.ActiveUser.UserName;
                    }
                    catch
                    {
                    }
                }
                return _CurrentUser;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var cu = value.ToString();
                return cu != CurrentUser ? Brushes.LightGreen : Brushes.LightPink;
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
