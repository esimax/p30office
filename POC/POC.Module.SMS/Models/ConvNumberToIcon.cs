using System;
using System.Windows.Data;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POC.Module.SMS.Models
{
    public class ConvPhoneCompanyTypeToIcon : IValueConverter
    {
        private static object _ImageHamrahAval;
        private static object _ImageIranCell;
        private static object _ImageRightel;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Enum)) return value;

            if (_ImageHamrahAval == null)
                _ImageHamrahAval = HelperImage.GetSpecialImageXX("_14_HamrahAval.png");

            if (_ImageIranCell == null)
                _ImageIranCell = HelperImage.GetSpecialImageXX("_14_Irancell.png");

            if (_ImageRightel == null)
                _ImageRightel = HelperImage.GetSpecialImageXX("_14_RighTel.png");

            switch ((EnumPhoneCompanyType)value)
            {
                case EnumPhoneCompanyType.Unknown:
                    return null;
                case EnumPhoneCompanyType.HamrahAval:
                    return _ImageHamrahAval;
                case EnumPhoneCompanyType.IranCell:
                    return _ImageIranCell;
                case EnumPhoneCompanyType.RighTel:
                    return _ImageRightel;
                case EnumPhoneCompanyType.Talia:
                    return null;
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
