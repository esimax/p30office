using System;
using System.Collections.Generic;
using System.Globalization;

namespace POL.Lib.Utils
{
    public class HelperGregorianCalendar
    {
        static HelperGregorianCalendar()
        {
            _Calendare = new GregorianCalendar();
        }

        private static Calendar _Calendare { get; }

        public static int GetYear(DateTime date)
        {
            return _Calendare.GetYear(date);
        }

        public static int GetMonth(DateTime date)
        {
            return _Calendare.GetMonth(date);
        }

        public static int GetDay(DateTime date)
        {
            return _Calendare.GetDayOfMonth(date);
        }

        public static int GetDaysInMonth(int year, int month)
        {
            return _Calendare.GetDaysInMonth(year, month);
        }

        public static string ToString(DateTime date, string format)
        {
            try
            {
                format = format.Replace("dddd", GetDayofWeekName(date));
                format = format.Replace("dd",
                    _Calendare.GetDayOfMonth(date).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'));
                format = format.Replace("MMMM", GetMonthName(_Calendare.GetMonth(date)));
                format = format.Replace("MM",
                    _Calendare.GetMonth(date).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'));
                format = format.Replace("yyyy", _Calendare.GetYear(date).ToString(CultureInfo.InvariantCulture));
                format = format.Replace("yy",
                    _Calendare.GetYear(date).ToString(CultureInfo.InvariantCulture).Substring(2).PadLeft(2, '0'));
                var h = _Calendare.GetHour(date);

                var h2 = h%12;

                if (h == 0) h = 12;
                if (h2 == 0) h2 = 12;
                format = format.Replace("HH", h.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'));
                format = format.Replace("hh", h2.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'));
                format = format.Replace("mm",
                    _Calendare.GetMinute(date).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'));
                format = format.Replace("ss",
                    _Calendare.GetSecond(date).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'));
                format = format.Replace("tt", h == h2 ? "ق.ظ" : "ب.ظ");
            }
            catch
            {
                return string.Empty;
            }
            return format;
        }

        public static string GetMonthName(int MonthNumber)
        {
            var rv = string.Empty;
            switch (MonthNumber)
            {
                case 1:
                    rv = "January";
                    break;
                case 2:
                    rv = "February";
                    break;
                case 3:
                    rv = "March";
                    break;
                case 4:
                    rv = "April";
                    break;
                case 5:
                    rv = "May";
                    break;
                case 6:
                    rv = "June";
                    break;
                case 7:
                    rv = "July";
                    break;
                case 8:
                    rv = "August";
                    break;
                case 9:
                    rv = "September";
                    break;
                case 10:
                    rv = "October";
                    break;
                case 11:
                    rv = "November";
                    break;
                case 12:
                    rv = "December";
                    break;
            }
            return rv;
        }

        public static string GetMonthName(DateTime dt)
        {
            return GetMonthName(_Calendare.GetMonth(dt));
        }

        public static string GetDayofWeekName(DateTime date)
        {
            var dow = _Calendare.GetDayOfWeek(date);
            return GetDayofWeekName(dow);
        }

        public static string GetDayofWeekName(DayOfWeek dayofweek)
        {
            var rv = string.Empty;
            switch (dayofweek)
            {
                case DayOfWeek.Saturday:
                    rv = "Saturday";
                    break;
                case DayOfWeek.Sunday:
                    rv = "Sunday";
                    break;
                case DayOfWeek.Monday:
                    rv = "Monday";
                    break;
                case DayOfWeek.Tuesday:
                    rv = "Tuesday";
                    break;
                case DayOfWeek.Wednesday:
                    rv = "Wednesday";
                    break;
                case DayOfWeek.Thursday:
                    rv = "Thursday";
                    break;
                case DayOfWeek.Friday:
                    rv = "Friday";
                    break;
            }
            return rv;
        }

        public static string GetDayofWeekShortName(DateTime date)
        {
            try
            {
                return GetDayofWeekName(date).Substring(0, 2);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetDayofWeekShortName(int dayNum)
        {
            try
            {
                return GetDayofWeekName((DayOfWeek) dayNum).Substring(0, 2);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static DateTime ConvertToGeoDate(int year, int month, int day)
        {
            return _Calendare.ToDateTime(year, month, day, 1, 1, 1, 1).Date;
        }

        public static List<DateTime> GetStartingMonthFromInterval(DateTime d1, DateTime d2)
        {
            var rv = new List<DateTime>();

            var dmax = GetMaxDate(d1, d2);
            var dmin = GetMinDate(d1, d2);

            var y0 = GetYear(dmin);
            var m0 = GetMonth(dmin);

            var dnew = ConvertToGeoDate(y0, m0, 1);
            while (dnew < dmax)
            {
                rv.Add(dnew);
                m0++;
                if (m0 > 12)
                {
                    m0 = 1;
                    y0++;
                }
                dnew = ConvertToGeoDate(y0, m0, 1);
            }
            rv.Add(dnew);
            return rv;
        }

        public static DateTime GetMaxDate(DateTime d1, DateTime d2)
        {
            return d1 > d2 ? new DateTime(d1.Ticks) : new DateTime(d2.Ticks);
        }

        public static DateTime GetMinDate(DateTime d1, DateTime d2)
        {
            return d1 > d2 ? new DateTime(d2.Ticks) : new DateTime(d1.Ticks);
        }
    }
}
