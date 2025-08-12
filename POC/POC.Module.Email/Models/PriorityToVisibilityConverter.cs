using System;
using System.Windows;
using System.Windows.Data;
using POL.Lib.Interfaces;

namespace POC.Module.Email.Models
{
    public class PriorityToVisibilityConverter : IValueConverter
    {
        public EnumEmailPriority VisiblePriority { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return Visibility.Collapsed;

                var priority = (EnumEmailPriority)value;
                if (priority == EnumEmailPriority.Normal) return Visibility.Collapsed;

                if (VisiblePriority <= EnumEmailPriority.High && priority <= EnumEmailPriority.High)
                    return Visibility.Visible;

                if (VisiblePriority >= EnumEmailPriority.Low && priority >= EnumEmailPriority.Low)
                    return Visibility.Visible;
            }
            catch
            {
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
