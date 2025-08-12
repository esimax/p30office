using System;
using System.Windows.Data;
using System.Windows.Media;

namespace POC.Module.Profile.Models
{
    public class ConvTemplateColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is double)
                {
                    var val = System.Convert.ToInt64(value);
                    var rv = Color.FromArgb((byte)(val >> 24), (byte)((val << 8) >> 24), (byte)((val << 16) >> 24),
                                            (byte)((val << 24) >> 24));
                    return new SolidColorBrush(rv);
                }
            }
            catch
            {
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
