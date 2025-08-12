using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POL.WPF.Controles.Conv
{
    public class ConvGlobalDate : IValueConverter
    {
        static ConvGlobalDate()
        {
            SelfStatic = new ConvGlobalDate();
        }

        #region Private Properties

        private string DateTimeFormatString
        {
            get { return Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern; }
        }

        #endregion

        #region Public Properties

        public EnumCalendarType DateType { get; set; }

        #endregion

        public static ConvGlobalDate SelfStatic { get; set; }

        #region Private Methods

        private Calendar GetCalendar()
        {
            Calendar pc = null;
            switch (DateType)
            {
                case EnumCalendarType.ApplicationSettings:
                    var agc = HelperLocalize.ApplicationCalendar;
                    if (agc == EnumCalendarType.Gregorian)
                        pc = new GregorianCalendar();
                    else if (agc == EnumCalendarType.Hijri)
                        pc = new HijriCalendar();
                    else if (agc == EnumCalendarType.Shamsi)
                        pc = new PersianCalendar();
                    else
                        pc = new PersianCalendar();
                    break;
                case EnumCalendarType.Shamsi:
                    pc = new PersianCalendar();
                    break;
                case EnumCalendarType.Hijri:
                    pc = new HijriCalendar();
                    break;
                case EnumCalendarType.Gregorian:
                    pc = new GregorianCalendar();
                    break;
            }
            return pc;
        }

        private object GetValidDate(DateTime d, object parameter)
        {
            try
            {
                var pc = GetCalendar();

                var sy = pc.GetYear(d).ToString(CultureInfo.InvariantCulture).PadLeft(4, '0');
                var sm = pc.GetMonth(d).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
                var sd = pc.GetDayOfMonth(d).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');


                var rv = string.IsNullOrWhiteSpace(parameter as string) ? DateTimeFormatString : parameter.ToString();
                if (rv.Contains("MMMM"))
                    rv = rv.Replace("MMMM", HelperPersianCalendar.GetMonthName(pc.GetMonth(d)));
                if (rv.Contains("MM"))
                    rv = rv.Replace("MM", sm);
                if (rv.Contains("dddd"))
                    rv = rv.Replace("MMMM", HelperPersianCalendar.GetDayofWeekName(pc.GetDayOfWeek(d)));
                if (rv.Contains("dd"))
                    rv = rv.Replace("dd", sd);
                if (rv.Contains("yyyy"))
                    rv = rv.Replace("yyyy", sy);


                rv = rv.Replace("yy", sy.Substring(2)).Replace("M", sm).Replace("d", sd)
                    .Replace("HH", d.Hour.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'))
                    .Replace("mm", d.Minute.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'))
                    .Replace("ss", d.Second.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'));

                return rv;
            }
            catch
            {
                return GetNotValidDate();
            }
        }

        private object GetNotValidDate()
        {
            var rv = DateTimeFormatString.Replace("yyyy", "y");
            rv = rv.Replace("yy", "y");
            rv = rv.Replace("MMMM", "M");
            rv = rv.Replace("MM", "M");
            rv = rv.Replace("dddd", "d");
            rv = rv.Replace("dd", "d");
            rv = rv.Replace("y", "0000").Replace("M", "00").Replace("d", "00");
            return rv;
        }

        #endregion

        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime)) return GetNotValidDate();
            var d = (DateTime) value;
            return GetValidDate(d, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var s = value.ToString();
                var ss = s.Split(new[] {Thread.CurrentThread.CurrentUICulture.DateTimeFormat.DateSeparator},
                    StringSplitOptions.RemoveEmptyEntries);

                var ss2 = DateTimeFormatString.Split(
                    new[] {Thread.CurrentThread.CurrentUICulture.DateTimeFormat.DateSeparator},
                    StringSplitOptions.RemoveEmptyEntries);

                var pc = GetCalendar();

                var id = 0;
                var im = 0;
                var iy = 0;

                for (var i = 0; i < 3; i++)
                {
                    if (ss2[i].Contains('d'))
                    {
                        id = System.Convert.ToInt32(ss[i]);
                    }
                    if (ss2[i].Contains('M'))
                    {
                        im = System.Convert.ToInt32(ss[i]);
                    }
                    if (ss2[i].Contains('y'))
                    {
                        iy = System.Convert.ToInt32(ss[i]);
                        if (iy < 100)
                        {
                            iy += pc.GetYear(DateTime.Now.AddYears(-30))/100*100;
                        }
                    }
                }

                var d = pc.ToDateTime(iy, im, id, 0, 0, 0, 0);
                return d;
            }
            catch
            {
            }
            return null;
        }

        #endregion
    }
}
