using System;
using System.Globalization;
using System.Windows.Data;
using POL.Lib.Utils;

namespace POL.Lib.Common
{
    public class ConvCallRecordRole : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                var ct = System.Convert.ToInt32(value);
                switch (ct)
                {
                    case 1:
                        return HelperImage.GetStandardImage16("_16_WhiteCircle.png");
                    case 2:
                        return HelperImage.GetStandardImage16("_16_YellowCircle.png");
                    case 3:
                        return HelperImage.GetStandardImage16("_16_RedCircle.png");
                    default:
                        return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
