using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace POL.WPF.Controles.Conv
{
    public class ConvBoolToBrush : IValueConverter
    {
        public ConvBoolToBrush()
        {
            TrueBrush = Brushes.White;
            FalseBrush = Brushes.Black;
        }

        public Brush TrueBrush { get; set; }
        public Brush FalseBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                var b = (bool) value;
                return b ? TrueBrush : FalseBrush;
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
