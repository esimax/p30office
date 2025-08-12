using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using DevExpress.Data.Mask;

namespace POL.WPF.DXControls.POLDateEdit
{
    public class MyDateTimeMaskFormatInfo : DateTimeMaskFormatInfo
    {
        protected readonly IList<DateTimeMaskFormatElement> InnerList2;

        public MyDateTimeMaskFormatInfo(string mask, DateTimeFormatInfo dateTimeFormatInfo)
            : base(mask, dateTimeFormatInfo)
        {
            var str = ExpandFormat(mask, dateTimeFormatInfo);
            InnerList2 = ParseFormatString(str, dateTimeFormatInfo);
            var fieldInfo = GetType().GetField("innerList", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
                fieldInfo.SetValue(this, InnerList2);
        }

        private static string ExpandFormat(string format, DateTimeFormatInfo info)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "G";
            }
            if (format.Length == 1)
            {
                switch (format[0])
                {
                    case 'D':
                        return info.LongDatePattern;

                    case 'F':
                        return info.FullDateTimePattern;

                    case 'G':
                        return info.ShortDatePattern + ' ' + info.LongTimePattern;

                    case 'M':
                    case 'm':
                        return info.MonthDayPattern;

                    case 'R':
                    case 'r':
                        return info.RFC1123Pattern;

                    case 'T':
                        return info.LongTimePattern;

                    case 's':
                        return info.SortableDateTimePattern;

                    case 't':
                        return info.ShortTimePattern;

                    case 'u':
                        return info.UniversalSortableDateTimePattern;

                    case 'y':
                    case 'Y':
                        return info.YearMonthPattern;

                    case 'd':
                        return info.ShortDatePattern;

                    case 'f':
                        return info.LongDatePattern + ' ' + info.ShortTimePattern;

                    case 'g':
                        return info.ShortDatePattern + ' ' + info.ShortTimePattern;
                }
            }
            if ((format.Length == 2) && (format[0] == '%'))
            {
                format = format.Substring(1);
            }
            return format;
        }

        private static int GetGroupLength(string mask)
        {
            for (var i = 1; i < mask.Length; i++)
            {
                if (mask[i] != mask[0])
                {
                    return i;
                }
            }
            return mask.Length;
        }

        private static IList<DateTimeMaskFormatElement> ParseFormatString(string mask,
            DateTimeFormatInfo dateTimeFormatInfo)
        {
            var list = new List<DateTimeMaskFormatElement>();
            var str = mask;
            var globalContext = new DateTimeMaskFormatGlobalContext();
            while (str.Length > 0)
            {
                DateTimeMaskFormatElement element;
                var groupLength = GetGroupLength(str);
                switch (str[0])
                {
                    case '/':
                        groupLength = 1;
                        element = new DateTimeMaskFormatElementNonEditable(str.Substring(0, 1), dateTimeFormatInfo,
                            DateTimePart.Date);
                        goto Label_0315;

                    case ':':
                        groupLength = 1;
                        element = new DateTimeMaskFormatElementNonEditable(str.Substring(0, 1), dateTimeFormatInfo,
                            DateTimePart.Time);
                        goto Label_0315;

                    case 'H':
                        element = new DateTimeMaskFormatElement_H24(str.Substring(0, groupLength), dateTimeFormatInfo);
                        goto Label_0315;

                    case '"':
                    case '\'':
                    {
                        var index = str.Replace(@"\\", "--").Replace(@"\" + str[0], "--").IndexOf(str[0], 1);
                        if (index <= 0)
                        {
                            throw new ArgumentException("Incorrect mask: closing quote expected");
                        }
                        var format = str.Substring(0, index + 1);
                        element =
                            new DateTimeMaskFormatElementLiteral(
                                DateTime.MinValue.ToString(format, dateTimeFormatInfo), dateTimeFormatInfo);
                        groupLength = index + 1;
                        goto Label_0315;
                    }
                    case 'd':
                        if (groupLength <= 2)
                        {
                            break;
                        }
                        element = new DateTimeMaskFormatElementNonEditable(str.Substring(0, groupLength),
                            dateTimeFormatInfo, DateTimePart.Date);
                        goto Label_0315;

                    case 'f':
                        if (groupLength > 7)
                        {
                            groupLength = 7;
                        }
                        element = groupLength > 3
                            ? new DateTimeMaskFormatElementNonEditable(str.Substring(0, groupLength), dateTimeFormatInfo,
                                DateTimePart.Time)
                            : new DateTimeMaskFormatElement_Millisecond(str.Substring(0, groupLength),
                                dateTimeFormatInfo);
                        goto Label_0315;

                    case 'g':
                        element = new DateTimeMaskFormatElementNonEditable(str.Substring(0, groupLength),
                            dateTimeFormatInfo, DateTimePart.Date);
                        goto Label_0315;

                    case 'h':
                        element = new DateTimeMaskFormatElement_h12(str.Substring(0, groupLength), dateTimeFormatInfo);
                        goto Label_0315;

                    case '\\':
                        if (str.Length < 2)
                        {
                            throw new ArgumentException(@"Incorrect mask: character expected after '\'");
                        }
                        element = new DateTimeMaskFormatElementLiteral(str.Substring(1, 1), dateTimeFormatInfo);
                        groupLength = 2;
                        goto Label_0315;

                    case 'M':
                        if (groupLength > 4)
                        {
                            groupLength = 4;
                        }
                        element = new MyDateTimeMaskFormatElement_Month(str.Substring(0, groupLength),
                            dateTimeFormatInfo, globalContext);
                        globalContext.Value.MonthProcessed = true;
                        goto Label_0315;

                    case 's':
                        element = new DateTimeMaskFormatElement_s(str.Substring(0, groupLength), dateTimeFormatInfo);
                        goto Label_0315;

                    case 't':
                        element = new DateTimeMaskFormatElement_AmPm(str.Substring(0, groupLength), dateTimeFormatInfo);
                        goto Label_0315;

                    case 'm':
                        element = new DateTimeMaskFormatElement_Min(str.Substring(0, groupLength), dateTimeFormatInfo);
                        goto Label_0315;

                    case 'y':
                        element = new MyDateTimeMaskFormatElement_Year(str.Substring(0, groupLength), dateTimeFormatInfo);
                        globalContext.Value.YearProcessed = true;
                        goto Label_0315;

                    case 'z':
                        element = new DateTimeMaskFormatElementNonEditable(str.Substring(0, groupLength),
                            dateTimeFormatInfo, DateTimePart.Both);
                        goto Label_0315;

                    default:
                        groupLength = 1;
                        element = new DateTimeMaskFormatElementLiteral(str.Substring(0, 1), dateTimeFormatInfo);
                        goto Label_0315;
                }
                element = new MyDateTimeMaskFormatElement_d(str.Substring(0, groupLength), dateTimeFormatInfo,
                    globalContext.Value);
                globalContext.Value.DayProcessed = true;
                Label_0315:
                list.Add(element);
                str = str.Substring(groupLength);
            }
            return list;
        }

        public class MyDateTimeMaskFormatElement_Year : DateTimeNumericRangeFormatElementEditable
        {
            public MyDateTimeMaskFormatElement_Year(string mask, DateTimeFormatInfo dateTimeFormatInfo)
                : base(mask, dateTimeFormatInfo, DateTimePart.Date)
            {
            }

            protected static Calendar CurrentCalendar
            {
                get { return CultureInfo.CurrentCulture.DateTimeFormat.Calendar; }
            }

            public override DateTime ApplyElement(int result, DateTime editedDateTime)
            {
                return CurrentCalendar.AddYears(editedDateTime, result - CurrentCalendar.GetYear(editedDateTime));
            }

            public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime)
            {
                return new MyDateTimeYearElementEditor(CurrentCalendar.GetYear(editedDateTime), Mask.Length,
                    DateTimeFormatInfo);
            }
        }
    }
}
