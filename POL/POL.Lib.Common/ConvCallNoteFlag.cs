using System;
using System.Globalization;
using System.Windows.Data;
using POL.Lib.Utils;

namespace POL.Lib.Common
{
    public class ConvCallNoteFlag : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                var ct = System.Convert.ToInt32(value);
                switch (ct)
                {
                    case 1:
                        return HelperImage.GetSpecialImage16("_16_Flag_Gray.png");
                    case 2:
                        return HelperImage.GetSpecialImage16("_16_Flag_Yellow.png");
                    case 3:
                        return HelperImage.GetSpecialImage16("_16_Flag_Red.png");
                    case 4:
                        return HelperImage.GetSpecialImage16("_16_Flag_Green.png");
                    case 5:
                        return HelperImage.GetSpecialImage16("_16_Flag_Blue.png");
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
