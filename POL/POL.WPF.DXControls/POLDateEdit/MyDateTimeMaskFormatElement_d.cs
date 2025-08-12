using System;
using System.Globalization;
using DevExpress.Data.Mask;

namespace POL.WPF.DXControls.POLDateEdit
{
    public class MyDateTimeMaskFormatElement_d : DateTimeNumericRangeFormatElementEditable
    {
        private DateTimeMaskFormatElementContext _Context;

        public MyDateTimeMaskFormatElement_d(string mask, DateTimeFormatInfo dateTimeFormatInfo,
            DateTimeMaskFormatElementContext context)
            : base(mask, dateTimeFormatInfo, DateTimePart.Date)
        {
            _Context = context;
        }

        protected static Calendar CurrentCalendar
        {
            get { return CultureInfo.CurrentCulture.DateTimeFormat.Calendar; }
        }

        public override DateTime ApplyElement(int result, DateTime editedDateTime)
        {
            var dateTime = CurrentCalendar.AddDays(editedDateTime,
                result - CurrentCalendar.GetDayOfMonth(editedDateTime));
            if (CurrentCalendar.GetDayOfMonth(dateTime) == result)
            {
                return dateTime;
            }
            if ((result == 0x1d) && (CurrentCalendar.GetMonth(editedDateTime) == 2) && _Context.MonthProcessed)
            {
                dateTime = editedDateTime;
                dateTime = MyDateTimeNumericRangeFormatElementEditable.ToLeapYear(dateTime);
                return CurrentCalendar.AddDays(dateTime, result - CurrentCalendar.GetDayOfMonth(dateTime));
            }
            return
                CurrentCalendar.AddDays(
                    CurrentCalendar.AddMonths(editedDateTime, 1 - CurrentCalendar.GetMonth(editedDateTime)),
                    result - CurrentCalendar.GetDayOfMonth(editedDateTime));
        }

        public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime)
        {
            var year = _Context.YearProcessed
                ? CurrentCalendar.GetYear(editedDateTime)
                : CurrentCalendar.GetYear(new DateTime(0x7d4, 1, 1));
            var month = _Context.MonthProcessed ? CurrentCalendar.GetMonth(editedDateTime) : 1;
            return new DateTimeNumericRangeElementEditor(CurrentCalendar.GetDayOfMonth(editedDateTime), 1,
                CurrentCalendar.GetDaysInMonth(year, month), Mask.Length == 1 ? 1 : 2, 2);
        }
    }
}
