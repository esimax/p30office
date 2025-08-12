using System;
using System.Collections.Generic;
using System.Globalization;

namespace POL.Lib.Utils
{
    public class HelperHijriCalendar
    {
        static HelperHijriCalendar()
        {
            _Calendare = new HijriCalendar();
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
                format = format.Replace("dd", _Calendare.GetDayOfMonth(date).ToString().PadLeft(2, '0'));
                format = format.Replace("MMMM", GetMonthName(_Calendare.GetMonth(date)));
                format = format.Replace("MM", _Calendare.GetMonth(date).ToString().PadLeft(2, '0'));
                format = format.Replace("yyyy", _Calendare.GetYear(date).ToString());
                format = format.Replace("yy", _Calendare.GetYear(date).ToString().Substring(2).PadLeft(2, '0'));
                var h = _Calendare.GetHour(date);

                var h2 = h%12;

                if (h == 0) h = 12;
                if (h2 == 0) h2 = 12;
                format = format.Replace("HH", h.ToString().PadLeft(2, '0'));
                format = format.Replace("hh", h2.ToString().PadLeft(2, '0'));
                format = format.Replace("mm", _Calendare.GetMinute(date).ToString().PadLeft(2, '0'));
                format = format.Replace("ss", _Calendare.GetSecond(date).ToString().PadLeft(2, '0'));
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
                    rv = "محرم";
                    break;
                case 2:
                    rv = "صفر";
                    break;
                case 3:
                    rv = "ربیع الاول";
                    break;
                case 4:
                    rv = "ربیع الثانی";
                    break;
                case 5:
                    rv = "جمادی الاول";
                    break;
                case 6:
                    rv = "جمادی الثانی";
                    break;
                case 7:
                    rv = "رجب";
                    break;
                case 8:
                    rv = "شعبان";
                    break;
                case 9:
                    rv = "رمضان";
                    break;
                case 10:
                    rv = "شوال";
                    break;
                case 11:
                    rv = "ذیقعده";
                    break;
                case 12:
                    rv = "ذیحجه";
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
                    rv = "السته";
                    break;
                case DayOfWeek.Sunday:
                    rv = "الاحد";
                    break;
                case DayOfWeek.Monday:
                    rv = "الاثنین";
                    break;
                case DayOfWeek.Tuesday:
                    rv = "الثلاثاء";
                    break;
                case DayOfWeek.Wednesday:
                    rv = "الاربعاء";
                    break;
                case DayOfWeek.Thursday:
                    rv = "الخمیس";
                    break;
                case DayOfWeek.Friday:
                    rv = "الجمعه";
                    break;
            }
            return rv;
        }

        public static string GetDayofWeekShortName(DateTime date)
        {
            var rv = string.Empty;
            var dow = _Calendare.GetDayOfWeek(date);
            switch (dow)
            {
                case DayOfWeek.Saturday:
                    rv = "ست";
                    break;
                case DayOfWeek.Sunday:
                    rv = "اح";
                    break;
                case DayOfWeek.Monday:
                    rv = "اث";
                    break;
                case DayOfWeek.Tuesday:
                    rv = "ثل";
                    break;
                case DayOfWeek.Wednesday:
                    rv = "ار";
                    break;
                case DayOfWeek.Thursday:
                    rv = "خم";
                    break;
                case DayOfWeek.Friday:
                    rv = "جم";
                    break;
            }
            return rv;
        }

        public static string GetDayofWeekShortName(int dayNum)
        {
            var rv = string.Empty;
            switch (dayNum)
            {
                case 0:
                    rv = "ست";
                    break;
                case 1:
                    rv = "اح";
                    break;
                case 2:
                    rv = "اث";
                    break;
                case 3:
                    rv = "ثل";
                    break;
                case 4:
                    rv = "ار";
                    break;
                case 5:
                    rv = "خم";
                    break;
                case 6:
                    rv = "جم";
                    break;
            }
            return rv;
        }

        public static DateTime ConvertToGeoDate(int year, int month, int day)
        {
            var rv = _Calendare.ToDateTime(year, month, day, 1, 1, 1, 1);
            return rv.Date;
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
