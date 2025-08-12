using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using POL.Lib.Interfaces;

namespace POL.WPF.Controles.Conv
{
    public class ConvStatusGeneralToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EnumStatusGeneral)
            {
                var b = (EnumStatusGeneral) value;
                switch (b)
                {
                    case EnumStatusGeneral.Invalid:
                        return Brushes.Tomato;
                    case EnumStatusGeneral.Checking:
                        return Brushes.Khaki;
                    case EnumStatusGeneral.OK:
                        return Brushes.LightGreen;
                    case EnumStatusGeneral.Undefined:
                        return Brushes.LightGray;
                }
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
