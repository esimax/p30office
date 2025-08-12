using System;
using System.Windows.Data;

namespace POC.Module.Attachment.Models
{
    public class ConvImportToText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ImportProductStructure)
            {
                var v = value as ImportProductStructure;
                switch (v.ErrorType)
                {
                    case EnumImportErrorType.None:
                        return string.Empty;
                    case EnumImportErrorType.ErrorInCode:
                        return "خطا در کد.";
                    case EnumImportErrorType.ErrorInTitle:
                        return "خطا در عنوان.";
                    case EnumImportErrorType.ErrorInPrice:
                        return "خطا در قیمت.";
                    case EnumImportErrorType.ErrorUnit:
                        return "خطا در واحد.";
                    case EnumImportErrorType.ErrorDuplicateTitle:
                        return "عنوان تکراری.";
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
