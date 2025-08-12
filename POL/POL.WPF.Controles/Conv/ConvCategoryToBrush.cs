using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Practices.Prism.Logging;

namespace POL.WPF.Controles.Conv
{
    public class ConvCategoryToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Category)
            {
                var b = (Category) value;
                switch (b)
                {
                    case Category.Exception:
                        return Brushes.Red;
                    case Category.Warn:
                        return Brushes.Purple;
                    case Category.Info:
                        return Brushes.DimGray;
                    case Category.Debug:
                        return Brushes.LightGray;
                }
            }
            return Brushes.DimGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
