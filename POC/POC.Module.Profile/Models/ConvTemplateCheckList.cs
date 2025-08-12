using System;
using System.Windows.Data;

namespace POC.Module.Profile.Models
{
    public class ConvTemplateCheckList : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is string)
                {
                    var stemp = value.ToString().Split(new[] {'|'});
                    return string.Join(Environment.NewLine, stemp);
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
