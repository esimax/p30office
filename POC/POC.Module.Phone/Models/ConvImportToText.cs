using System;
using System.Windows.Data;

namespace POC.Module.Phone.Models
{
    public class ConvImportToText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ImportPhoneStructure)
            {
                var v = value as ImportPhoneStructure;
                switch (v.ErrorType)
                {
                    case EnumImportErrorType.None:
                        return string.Empty;
                    case EnumImportErrorType.ErrorInContactTitle:
                        return "خطا در عنوان پرونده.";
                    case EnumImportErrorType.ErrorDuplicateNumber:
                        return "شماره تكراری می باشد.";
                    case EnumImportErrorType.ErrorInvalidNumber:
                        return "شماره معتبر نمی باشد.";
                    case EnumImportErrorType.ErrorInContactCode:
                        return "خطا در كد پرونده.";
                    case EnumImportErrorType.ErrorDuplicateNumberInExcel:
                        return "شماره در فایل اكسل تكراری می باشد.";
                    default:
                        return "خطای نامشخص.";
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
