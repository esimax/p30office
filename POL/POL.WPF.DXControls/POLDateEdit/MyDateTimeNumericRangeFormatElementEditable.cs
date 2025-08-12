using System;
using System.Globalization;
using DevExpress.Data.Mask;

namespace POL.WPF.DXControls.POLDateEdit
{
    public abstract class MyDateTimeNumericRangeFormatElementEditable : DateTimeMaskFormatElementEditable
    {
        protected MyDateTimeNumericRangeFormatElementEditable(string mask, DateTimeFormatInfo dateTimeFormatInfo,
            DateTimePart dateTimePart)
            : base(mask, dateTimeFormatInfo, dateTimePart)
        {
        }

        protected static Calendar CurrentCalendar
        {
            get { return CultureInfo.CurrentCulture.DateTimeFormat.Calendar; }
        }

        public static DateTime ToLeapYear(DateTime dateTime)
        {
            var time = dateTime;
            var num = 1;
            while (!CurrentCalendar.IsLeapYear(time.Year))
            {
                if (time.Year == DateTime.MaxValue.Year)
                {
                    num = -1;
                }
                time = time.AddYears(num);
            }
            return time;
        }
    }
}
