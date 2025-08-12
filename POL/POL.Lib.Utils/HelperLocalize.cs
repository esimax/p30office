using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using POL.Lib.Interfaces;
using FlowDirection = System.Windows.FlowDirection;

namespace POL.Lib.Utils
{
    public class HelperLocalize
    {
        public static string ApplicationDefKeyboardLeyout { get; set; }
        public static string ApplicationRTLKeyboardLeyout { get; set; }
        public static string ApplicationLTRKeyboardLeyout { get; set; }
        public static FlowDirection ApplicationFlowDirection { get; set; }
        public static string ApplicationFontName { get; set; }
        public static double ApplicationFontSize { get; set; }
        public static EnumCalendarType ApplicationCalendar { get; set; }

        public static string ApplicationDateTimeFormat
        {
            get { return Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern; }
        }

        public static string ApplicationDateSeparator
        {
            get { return Thread.CurrentThread.CurrentUICulture.DateTimeFormat.DateSeparator; }
        }


        public static void SetLanguageToDefault()
        {
            foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
            {
                if (lang.LayoutName == ApplicationDefKeyboardLeyout)
                {
                    InputLanguage.CurrentInputLanguage = lang;
                    break;
                }
            }
        }

        public static void SetLanguageToRTL()
        {
            foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
            {
                if (lang.LayoutName == ApplicationRTLKeyboardLeyout)
                {
                    InputLanguage.CurrentInputLanguage = lang;
                    break;
                }
            }
        }

        public static void SetLanguageToLTR()
        {
            foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
            {
                if (lang.LayoutName == ApplicationLTRKeyboardLeyout)
                {
                    InputLanguage.CurrentInputLanguage = lang;
                    break;
                }
            }
        }

        public static InputLanguage GetFarsiLanguage()
        {
            return
                InputLanguage.InstalledInputLanguages.Cast<InputLanguage>()
                    .FirstOrDefault(lang => (lang.LayoutName == "Farsi") || (lang.LayoutName == "Persian"));
        }

        public static InputLanguage GetEnglishLanguage()
        {
            return
                InputLanguage.InstalledInputLanguages.Cast<InputLanguage>()
                    .FirstOrDefault(lang => (lang.LayoutName == "US") || (lang.LayoutName == "UK"));
        }

        public static InputLanguage GetCurrentLanguage()
        {
            return InputLanguage.CurrentInputLanguage;
        }

        public static void SetLanguage(InputLanguage il)
        {
            InputLanguage.CurrentInputLanguage = il;
        }

        public static void SetFarsiLanguage()
        {
            var l = GetFarsiLanguage();
            if (l != null)
            {
                InputLanguage.CurrentInputLanguage = l;
            }
        }

        public static void SetEnglishLanguage()
        {
            var l = GetEnglishLanguage();
            if (l != null)
            {
                InputLanguage.CurrentInputLanguage = l;
            }
        }

        public static void ChangeCultureToPersian()
        {
            var calture = new CultureInfo("fa-IR");
            var info = calture.DateTimeFormat;
            info.AbbreviatedDayNames = new[] {"ى", "د", "س", "چ", "پ", "ج", "ش"};
            info.DayNames = new[] {"یكشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه"};
            info.AbbreviatedMonthNames = new[]
            {
                "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان",
                "آذر", "دی", "بهمن", "اسفند", ""
            };
            info.MonthNames = new[]
            {
                "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی",
                "بهمن", "اسفند", ""
            };
            info.AMDesignator = "ق.ظ";
            info.PMDesignator = "ب.ظ";
            info.ShortDatePattern = "yyyy/MM/dd";
            info.FirstDayOfWeek = DayOfWeek.Saturday;
            var PersianCal = new PersianCalendar();
            var fieldInfo = typeof (DateTimeFormatInfo).GetField("calendar",
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (fieldInfo != null)
                fieldInfo
                    .SetValue(info, PersianCal);
            try
            {
                var field = typeof (DateTimeFormatInfo).GetField("m_cultureTableRecord",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    var obj =
                        field.GetValue(info);
                    obj.GetType()
                        .GetMethod("UseCurrentCalendar", BindingFlags.NonPublic | BindingFlags.Instance)
                        .Invoke(
                            obj,
                            new[]
                            {
                                PersianCal.GetType().GetProperty("ID", BindingFlags.NonPublic | BindingFlags.Instance).
                                    GetValue(PersianCal, null)
                            });
                }
            }
            catch
            {
            }
            typeof (CultureInfo).GetField("calendar",
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).
                SetValue(calture, PersianCal);
            Thread.CurrentThread.CurrentCulture = calture;
            Thread.CurrentThread.CurrentUICulture = calture;
            CultureInfo.CurrentCulture.DateTimeFormat = info;
            CultureInfo.CurrentUICulture.DateTimeFormat = info;
        }


        public static DateTime? StringToDateTime(string value, EnumCalendarType dt)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            try
            {
                var ss = value.Split(new[] {ApplicationDateSeparator}, StringSplitOptions.RemoveEmptyEntries);

                var ss2 = ApplicationDateTimeFormat.Split(new[] {ApplicationDateSeparator},
                    StringSplitOptions.RemoveEmptyEntries);

                var pc = GetCalendar(dt);

                var id = 0;
                var im = 0;
                var iy = 0;

                for (var i = 0; i < 3; i++)
                {
                    if (ss2[i].Contains('d'))
                    {
                        id = Convert.ToInt32(ss[i]);
                    }
                    if (ss2[i].Contains('M'))
                    {
                        im = Convert.ToInt32(ss[i]);
                    }
                    if (ss2[i].Contains('y'))
                    {
                        iy = Convert.ToInt32(ss[i]);
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
                throw new InvalidCastException();
            }
        }

        public static string DateTimeToString(DateTime d, EnumCalendarType dtt, string format)
        {
            try
            {
                var pc = GetCalendar(dtt);

                var sy = pc.GetYear(d).ToString().PadLeft(4, '0');
                var sm = pc.GetMonth(d).ToString().PadLeft(2, '0');
                var sd = pc.GetDayOfMonth(d).ToString().PadLeft(2, '0');
                var dddd = GetDayName(dtt, pc.GetDayOfWeek(d));
                var MMMM = GetMonthName(dtt, pc.GetMonth(d));


                var rv = format;
                if (!rv.Contains("MM"))
                    rv = rv.Replace("M", "MM");
                if (!rv.Contains("dd"))
                    rv = rv.Replace("d", "dd");

                if (rv.Contains("dddd"))
                    rv = rv.Replace("dddd", dddd);
                if (rv.Contains("dd"))
                    rv = rv.Replace("dd", sd);
                if (rv.Contains("yyyy"))
                    rv = rv.Replace("yyyy", sy);
                if (rv.Contains("MMMM"))
                    rv = rv.Replace("MMMM", MMMM);
                if (rv.Contains("MM"))
                    rv = rv.Replace("MM", sm);


                rv = rv.Replace("yy", sy.Substring(2));

                return rv;
            }
            catch
            {
                return null;
            }
        }

        public static string GetMonthName(EnumCalendarType dtt, int monthnumber)
        {
            switch (dtt)
            {
                case EnumCalendarType.Shamsi:
                    return HelperPersianCalendar.GetMonthName(monthnumber);
                case EnumCalendarType.Hijri:
                    return HelperHijriCalendar.GetMonthName(monthnumber);
                case EnumCalendarType.Gregorian:
                    return HelperGregorianCalendar.GetMonthName(monthnumber);
            }
            return string.Empty;
        }

        public static string GetDayName(EnumCalendarType dtt, DayOfWeek dayOfWeek)
        {
            switch (dtt)
            {
                case EnumCalendarType.Shamsi:
                    return HelperPersianCalendar.GetDayofWeekName(dayOfWeek);
                case EnumCalendarType.Hijri:
                    return HelperHijriCalendar.GetDayofWeekName(dayOfWeek);
                case EnumCalendarType.Gregorian:
                    return HelperGregorianCalendar.GetDayofWeekName(dayOfWeek);
            }
            return string.Empty;
        }

        public static Calendar GetCalendar(EnumCalendarType dtt)
        {
            switch (dtt)
            {
                case EnumCalendarType.Hijri:
                    return new HijriCalendar();
                case EnumCalendarType.Gregorian:
                    return new GregorianCalendar();
                default:
                    return new PersianCalendar();
            }
        }

        [DllImport("Kernel32.dll")]
        private static extern bool SetLocalTime(ref SYSTEMTIME Time);

        public static bool SetSystemDateTime(DateTime datetime)
        {
            var rv = false;
            try
            {
                var st = new SYSTEMTIME();
                st.FromDateTime(datetime);
                SetLocalTime(ref st);
                rv = true;
            }
            catch
            {
            }
            return rv;
        }


        public static string GetNumberAsText(int number)
        {
            var i = number;
            var pri = string.Empty;
            if (number < 0)
            {
                pri = "منفی ";
                i = -number;
            }
            if (i == 0) return "صفر";
            if (i < 10) return string.Format("{0}{1}", pri, Get10(i));
            if (i == 10) return string.Format("{0}ده", pri);
            if (i < 20) return string.Format("{0}{1}", pri, Get20(i));
            if (i < 100)
            {
                var i1 = i%10;
                var i2 = i/10;
                return string.Format("{0}{1} و {2}", pri, GetTenth(i2), Get10(i1));
            }
            return "بزرگتر یا مساوی صد";
        }

        private static string GetTenth(int i)
        {
            switch (i)
            {
                case 2:
                    return "بیست";
                case 3:
                    return "سی";
                case 4:
                    return "چهل";
                case 5:
                    return "پنجاه";
                case 6:
                    return "شصت";
                case 7:
                    return "هفتاد";
                case 8:
                    return "هشتاد";
                case 9:
                    return "نود";
            }
            return string.Empty;
        }

        private static string Get20(int i)
        {
            switch (i)
            {
                case 11:
                    return "یازده";
                case 12:
                    return "دوازده";
                case 13:
                    return "سیزده";
                case 14:
                    return "چهارده";
                case 15:
                    return "پانزده";
                case 16:
                    return "شانزده";
                case 17:
                    return "هفده";
                case 18:
                    return "هجده";
                case 19:
                    return "نوزده";
            }
            return string.Empty;
        }

        private static string Get10(int i)
        {
            switch (i)
            {
                case 1:
                    return "یك";
                case 2:
                    return "دو";
                case 3:
                    return "سه";
                case 4:
                    return "چهار";
                case 5:
                    return "پنج";
                case 6:
                    return "شش";
                case 7:
                    return "هفت";
                case 8:
                    return "هشت";
                case 9:
                    return "نه";
            }
            return string.Empty;
        }


        public static DateTime ConvertFromDateTimeOffset(DateTimeOffset dateTime)
        {
            if (dateTime.Offset.Equals(TimeSpan.Zero))
                return dateTime.UtcDateTime;
            if (dateTime.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(dateTime.DateTime)))
                return DateTime.SpecifyKind(dateTime.DateTime, DateTimeKind.Local);
            return dateTime.DateTime;
        }

        #region Nested type: SYSTEMTIME

        private struct SYSTEMTIME
        {
            public ushort wDay;
            public ushort wDayOfWeek;
            public ushort wHour;
            public ushort wMilliseconds;
            public ushort wMinute;
            public ushort wMonth;
            public ushort wSecond;
            public ushort wYear;

            public void FromDateTime(DateTime time)
            {
                wYear = (ushort) time.Year;
                wMonth = (ushort) time.Month;
                wDayOfWeek = (ushort) time.DayOfWeek;
                wDay = (ushort) time.Day;
                wHour = (ushort) time.Hour;
                wMinute = (ushort) time.Minute;
                wSecond = (ushort) time.Second;
                wMilliseconds = (ushort) time.Millisecond;
            }

            public DateTime ToDateTime()
            {
                return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);
            }

            public static DateTime ToDateTime(SYSTEMTIME time)
            {
                return time.ToDateTime();
            }
        }

        #endregion
    }
}
