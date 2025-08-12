using System;
using System.Collections;
using System.Globalization;
using System.Reflection;

namespace POL.WPF.DXControls.POLDateEdit
{
    public class PersianCulture : CultureInfo
    {
        static PersianCulture()
        {
            PersianCalendar = new PersianCalendar();
        }

        public PersianCulture()
            : base("fa-ir")
        {
            Initialize();
        }

        public PersianCulture(bool useUserOverride)
            : base("fa-ir", useUserOverride)
        {
            Initialize();
        }

        public static PersianCalendar PersianCalendar { get; }

        private void Initialize()
        {
            DateTimeFormat.FirstDayOfWeek = DayOfWeek.Saturday;
            DateTimeFormat.DayNames = new[] {"یكشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه"};
            DateTimeFormat.AbbreviatedDayNames = new[]
            {"یكشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه"};
            DateTimeFormat.ShortestDayNames = new[] {"ی", "د", "س", "چ", "پ", "ج", "ش"};
            DateTimeFormat.MonthNames = new[]
            {"فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند", ""};
            DateTimeFormat.AbbreviatedMonthNames = new[]
            {"فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند", ""};
            DateTimeFormat.AbbreviatedMonthGenitiveNames = new[]
            {"فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند", ""};
            DateTimeFormat.MonthGenitiveNames = new[]
            {"فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند", ""};
            DateTimeFormat.LongDatePattern = "dddd، d MMMM، yyyy";
            DateTimeFormat.MonthDayPattern = "d MMMM";
            DateTimeFormat.YearMonthPattern = "MMMM، yyyy";
            DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";

            var f = typeof (DateTimeFormatInfo).GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
            if (f != null) f.SetValue(DateTimeFormat, PersianCalendar);
            f = typeof (DateTimeFormatInfo).GetField("optionalCalendars", BindingFlags.NonPublic | BindingFlags.Instance);
            if (f != null)
            {
                var opCals = new ArrayList(f.GetValue(DateTimeFormat) as int[]) {0x16};
                f.SetValue(DateTimeFormat, opCals.ToArray(typeof (int)));
            }
            f = typeof (PersianCulture).GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
            if (f != null) f.SetValue(this, PersianCalendar);
        }
    }
}
