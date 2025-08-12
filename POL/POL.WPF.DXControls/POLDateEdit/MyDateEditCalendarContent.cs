using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using DevExpress.Xpf.Editors.Popups.Calendar;

namespace POL.WPF.DXControls.POLDateEdit
{
    public class MyDateEditCalendarContent : DateEditCalendarContent
    {
        protected static Calendar CurrentCalendar
        {
            get { return CultureInfo.CurrentCulture.DateTimeFormat.Calendar; }
        }

        protected override void CalcFirstVisibleDate()
        {
            SetFirstVisibleDate(GetFirstVisibleDate(DateTime));
        }

        protected new DateTime GetFirstVisibleDate(DateTime value)
        {
            var firstMonthDate = GetFirstMonthDate(value);
            var span = TimeSpan.FromDays(-GetFirstDayOffset(firstMonthDate));
            if (firstMonthDate.Ticks + span.Ticks < 0L)
            {
                return DateTime.MinValue;
            }
            try
            {
                return firstMonthDate + span;
            }
            catch (ArgumentOutOfRangeException)
            {
                return GetMinValue(Calendar);
            }
        }

        protected new static DateTime GetFirstMonthDate(DateTime value)
        {
            var year = CurrentCalendar.GetYear(value);
            var month = CurrentCalendar.GetMonth(value);
            return CurrentCalendar.ToDateTime(year, month, 1, value.Hour, value.Minute, value.Second, value.Millisecond);
        }

        protected internal static DateTime GetMinValue(DateEditCalendar calendar)
        {
            if (calendar.MinValue.HasValue)
            {
                return calendar.MinValue.Value;
            }
            return CurrentCalendar.MinSupportedDateTime;
        }

        protected internal static DateTime GetMaxValue(DateEditCalendar calendar)
        {
            if (calendar.MaxValue.HasValue)
            {
                return calendar.MaxValue.Value;
            }
            return CurrentCalendar.MaxSupportedDateTime;
        }

        protected override void CreateYearsGroupCellInfo(int row, int col)
        {
            var num = CurrentCalendar.GetYear(DateTime)/100*100 - 10;
            var year = num + (row*4 + col)*10;
            if ((year < 0) || (year >= 0x2710)) return;
            var num3 = year + 9;
            if (year == 0) year = 1;
            var current = CurrentCalendar.ToDateTime(year, 1, 1, 0, 0, 0, 0, 0);
            AddCellInfo(row, col, current,
                string.Format("{0}-\n{1}", current.ToString("yyyy", CultureInfo.CurrentCulture),
                    CurrentCalendar.ToDateTime(num3, 1, 1, 0, 0, 0, 0, 0).ToString("yyyy", CultureInfo.CurrentCulture)),
                (current < GetMinValue(Calendar)) && (num3 < CurrentCalendar.GetYear(GetMinValue(Calendar))) ||
                (current > GetMaxValue(Calendar)));
        }

        protected override void CreateMonthCellInfo(int row, int col)
        {
            var current = CurrentCalendar.ToDateTime(CurrentCalendar.GetYear(DateTime), 1 + row*4 + col, 1, 0, 0, 0, 0);
            AddCellInfo(row, col, current, GetMonthName(CurrentCalendar.GetMonth(current)),
                (current > GetMaxValue(Calendar)) ||
                ((current < GetMinValue(Calendar)) &&
                 (CurrentCalendar.GetMonth(current) < CurrentCalendar.GetMonth(GetMinValue(Calendar)))));
        }

        protected override void CreateYearCellInfo(int row, int col)
        {
            var num = CurrentCalendar.GetYear(DateTime)/10*10 - 1;
            var year = num + row*4 + col;
            if ((year <= 0) || (year >= 0x2710)) return;
            var current = CurrentCalendar.ToDateTime(year, 1, 1, 0, 0, 0, 0);
            AddCellInfo(row, col, current, current.ToString("yyyy", CultureInfo.CurrentCulture),
                (current < GetMinValue(Calendar)) &&
                (CurrentCalendar.GetYear(current) < CurrentCalendar.GetYear(GetMinValue(Calendar))) ||
                (current > GetMaxValue(Calendar)));
        }

        protected override void CalcDayNumberCells()
        {
            var minValue = GetMinValue(Calendar);
            for (var i = 0; i < 6; i++)
            {
                var num3 = i == 0 ? FirstCellIndex : 0;
                for (var j = 0; j < 7; j++)
                {
                    if ((i == 0) && (j < num3))
                    {
                        AddCellInfo(i, j);
                    }
                    else
                    {
                        try
                        {
                            minValue = FirstVisibleDate.AddDays(i*7 + j - FirstCellIndex);
                        }
                        catch
                        {
                            i = 6;
                            break;
                        }
                        AddCellInfo(i, j, minValue,
                            CurrentCalendar.GetDayOfMonth(minValue).ToString(CultureInfo.InvariantCulture),
                            !CanAddDate(minValue));
                    }
                }
                if (Calendar.ShowWeekNumbers && (minValue != Calendar.MinValue))
                {
                    AddWeekNumber(i, minValue);
                }
            }
        }

        protected override bool CanAddDate(DateTime date)
        {
            var time = CurrentCalendar.ToDateTime(CurrentCalendar.GetYear(date), CurrentCalendar.GetMonth(date),
                CurrentCalendar.GetDayOfMonth(date), 0x17, 0x3b, 0x3b, 0, 0);
            var minValue = Calendar.MinValue;
            if (minValue.HasValue && (time < minValue.GetValueOrDefault())) return false;
            var time2 = CurrentCalendar.ToDateTime(CurrentCalendar.GetYear(date), CurrentCalendar.GetMonth(date),
                CurrentCalendar.GetDayOfMonth(date), 0, 0, 0, 0);
            var maxValue = Calendar.MaxValue;
            return !maxValue.HasValue || (time2 <= maxValue.GetValueOrDefault());
        }

        protected override void UpdateMonthInfoCell(DependencyObject obj, DateTime current)
        {
            if (Calendar != null)
            {
                if (current < GetMinValue(Calendar))
                    return;
                if (CurrentCalendar.GetMonth(current) != CurrentCalendar.GetMonth(DateTime))
                {
                    DateEditCalendar.SetCellInactive(obj, true);
                }
                var calendarTransferPublic = ((MyDateEditCalendar) Calendar).CalendarTransferPublic;
                var pi = calendarTransferPublic.GetType()
                    .GetProperty("HasExecutingAnimations",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty);
                var calendarTransferPublicHasExecutingAnimations = (bool) pi.GetValue(calendarTransferPublic, null);
                if (!calendarTransferPublicHasExecutingAnimations)
                {
                    if ((CurrentCalendar.GetMonth(current) == CurrentCalendar.GetMonth(DateTime.Now)) &&
                        (CurrentCalendar.GetYear(current) == CurrentCalendar.GetMonth(DateTime.Now)) &&
                        (CurrentCalendar.GetDayOfMonth(current) == CurrentCalendar.GetDayOfMonth(DateTime.Now)))
                    {
                        DateEditCalendar.SetCellToday(obj, true);
                    }
                    if ((CurrentCalendar.GetMonth(current) == CurrentCalendar.GetMonth(DateTime)) &&
                        (CurrentCalendar.GetYear(current) == CurrentCalendar.GetYear(DateTime)) &&
                        (CurrentCalendar.GetDayOfMonth(current) == CurrentCalendar.GetDayOfMonth(DateTime)))
                    {
                        DateEditCalendar.SetCellFocused(obj, true);
                    }
                }
            }
        }

        protected override void UpdateYearInfoCell(DependencyObject obj, DateTime current)
        {
            if ((CurrentCalendar.GetMonth(current) == CurrentCalendar.GetMonth(DateTime)) &&
                (CurrentCalendar.GetYear(current) == CurrentCalendar.GetYear(DateTime)))
            {
                DateEditCalendar.SetCellFocused(obj, true);
            }
        }

        protected override void UpdateYearsGroupInfoCell(DependencyObject obj, DateTime current)
        {
            if (CurrentCalendar.GetYear(current)/100 != CurrentCalendar.GetYear(DateTime)/100)
            {
                DateEditCalendar.SetCellInactive(obj, true);
            }
            if (CurrentCalendar.GetYear(current)/10 == CurrentCalendar.GetYear(DateTime)/10)
            {
                DateEditCalendar.SetCellFocused(obj, true);
            }
        }

        protected override void UpdateYearsInfoCell(DependencyObject obj, DateTime current)
        {
            if (CurrentCalendar.GetYear(current)/10 != CurrentCalendar.GetYear(DateTime)/10)
            {
                DateEditCalendar.SetCellInactive(obj, true);
            }
            if (CurrentCalendar.GetYear(current) == CurrentCalendar.GetYear(DateTime))
            {
                DateEditCalendar.SetCellFocused(obj, true);
            }
        }

        protected override string GetCurrentDateText()
        {
            if (State == DateEditCalendarState.Month)
            {
                return string.Format(CultureInfo.CurrentCulture, "{0:MMMM yyyy}", DateTime);
            }
            if (State == DateEditCalendarState.Year)
            {
                return DateTime.ToString("yyyy", CultureInfo.CurrentCulture);
            }
            if (State == DateEditCalendarState.Years)
            {
                return string.Format(CultureInfo.CurrentCulture, "{0:yyyy}-{1:yyyy}",
                    CurrentCalendar.ToDateTime(Math.Max(CurrentCalendar.GetYear(DateTime)/10*10, 1), 1, 1, 0, 0, 0, 0),
                    CurrentCalendar.ToDateTime(CurrentCalendar.GetYear(DateTime)/10*10 + 9, 1, 1, 0, 0, 0, 0));
            }
            return string.Format(CultureInfo.CurrentCulture, "{0:yyyy}-{1:yyyy}",
                CurrentCalendar.ToDateTime(Math.Max(CurrentCalendar.GetYear(DateTime)/100*100, 1), 1, 1, 0, 0, 0, 0),
                CurrentCalendar.ToDateTime(CurrentCalendar.GetYear(DateTime)/100*100 + 0x63, 1, 1, 0, 0, 0, 0));
        }

    }
}
