using System;
using System.Windows.Data;


namespace POC.Module.Profile.Models
{
    public class ConvTemplateBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is int)
                {
                    var i = (int)value;
                    if (i == 0) return false;
                    if (i == 1) return true;
                }
            }
            catch
            {
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


