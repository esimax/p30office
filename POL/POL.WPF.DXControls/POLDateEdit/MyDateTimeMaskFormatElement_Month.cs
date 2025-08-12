using System;
using System.Globalization;
using DevExpress.Data.Mask;

namespace POL.WPF.DXControls.POLDateEdit
{
    public class MyDateTimeMaskFormatElement_Month : DateTimeNumericRangeFormatElementEditable
    {
        private readonly DateTimeMaskFormatElementContext context;
        private string[] _MonthNames;
        private DateTimeMaskFormatGlobalContext monthNamesDeterminator;

        public MyDateTimeMaskFormatElement_Month(string mask, DateTimeFormatInfo dateTimeFormatInfo,
            DateTimeMaskFormatGlobalContext globalContext)
            : base(mask, dateTimeFormatInfo, DateTimePart.Date)
        {
            monthNamesDeterminator = globalContext;
            context = globalContext.Value;
        }

        private static Calendar CurrentCalendar
        {
            get { return CultureInfo.CurrentCulture.DateTimeFormat.Calendar; }
        }

        private string[] MonthNames
        {
            get
            {
                if (monthNamesDeterminator != null)
                {
                    if (Mask.Length == 3)
                    {
                        _MonthNames = monthNamesDeterminator.Value.DayProcessed
                            ? DateTimeFormatInfo.AbbreviatedMonthGenitiveNames
                            : DateTimeFormatInfo.AbbreviatedMonthNames;
                    }
                    else if (Mask.Length > 3)
                    {
                        _MonthNames = monthNamesDeterminator.Value.DayProcessed
                            ? DateTimeFormatInfo.MonthGenitiveNames
                            : DateTimeFormatInfo.MonthNames;
                    }
                    else
                    {
                        _MonthNames = null;
                    }
                    monthNamesDeterminator = null;
                }
                return _MonthNames;
            }
        }

        public override DateTime ApplyElement(int result, DateTime editedDateTime)
        {
            var dateTime = CurrentCalendar.AddMonths(editedDateTime, result - CurrentCalendar.GetMonth(editedDateTime));
            if ((result == 2) && (CurrentCalendar.GetDayOfMonth(editedDateTime) == 0x1d) &&
                (CurrentCalendar.GetDayOfMonth(editedDateTime) != CurrentCalendar.GetDayOfMonth(dateTime)) &&
                context.DayProcessed && !context.YearProcessed)
            {
                dateTime = editedDateTime;
                dateTime = MyDateTimeNumericRangeFormatElementEditable.ToLeapYear(dateTime);

                dateTime = CurrentCalendar.AddMonths(dateTime, result - CurrentCalendar.GetDayOfMonth(dateTime));
            }
            return dateTime;
        }

        public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime)
        {
            return new DateTimeMonthElementEditor(CurrentCalendar.GetMonth(editedDateTime), Mask.Length == 2 ? 2 : 1,
                MonthNames);
        }

        public override string Format(DateTime formattedDateTime)
        {
            if (MonthNames != null)
            {
                var month = CurrentCalendar.GetMonth(formattedDateTime);
                return MonthNames[month - 1];
            }
            return base.Format(formattedDateTime);
        }
    }
}
