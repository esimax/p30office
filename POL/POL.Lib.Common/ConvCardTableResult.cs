using System;
using System.Globalization;
using System.Windows.Data;

namespace POL.Lib.Common
{
    public class ConvCardTableResult : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var res = System.Convert.ToInt32(value);
                switch (res)
                {
                    case 0:
                        return "بدون نتیجه";
                    case 1:
                        return "در حال اجرا";
                    case 2:
                        return "انجام شد";
                    case 3:
                        return "لغو شد";
                    case 4:
                        return "ارجاع شد";
                    case 5:
                        return "انجام نشد";
                }
            }
            catch
            {
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
