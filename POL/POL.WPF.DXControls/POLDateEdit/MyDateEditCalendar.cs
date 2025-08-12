using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Editors.Popups.Calendar;
using Calendar = System.Globalization.Calendar;

namespace POL.WPF.DXControls.POLDateEdit
{
    public class MyDateEditCalendar : DateEditCalendar
    {
        public MyDateEditCalendar()
        {
            MinValue = CurrentCalendar.AddMonths(CurrentCalendar.MinSupportedDateTime, 6);
        }

        public DateEditCalendarTransferControl CalendarTransferPublic
        {
            get { return CalendarTransfer; }
        }

        protected static Calendar CurrentCalendar
        {
            get
            {
                return 
                    CultureInfo.CurrentCulture.DateTimeFormat.Calendar;
            }
        }

        protected override void SetNewContent(DateTime dt, DateEditCalendarState state, ControlTemplate template,
            DateEditCalendarTransferType transferType)
        {
            var fi = typeof (DateEditCalendar).GetField("prevContent", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi != null) fi.SetValue(this, ActiveContent);
            if (CalendarTransfer != null)
            {
                var content = new MyDateEditCalendarContent {FlowDirection = FlowDirection.LeftToRight};

                CalendarTransfer.TransferType = transferType;
                content.State = state;
                content.Template = template;
                content.DateTime = dt;
                CalendarTransfer.Content = content;
                if (LeftArrowButton != null)
                {
                    LeftArrowButton.IsEnabled = CanShiftLeft(dt);
                }
                if (RightArrowButton != null)
                {
                    RightArrowButton.IsEnabled = CanShiftRight(dt);
                }
            }
        }

        protected override void OnMonthCellButtonClick(Button button)
        {
            var date = (DateTime) GetDateTime(button);
            SetMonth(date);
            SetNewContent(DateTime, DateEditCalendarState.Month, MonthInfoTemplate, DateEditCalendarTransferType.ZoomIn);
        }

        private int GetRoundDay(int year, int month, int day)
        {
            var num = CurrentCalendar.GetDaysInMonth(year, month);
            if (day <= num)
            {
                return day;
            }
            return num;
        }

        protected new void SetMonth(DateTime newDateTime)
        {
            DateTime = CurrentCalendar.ToDateTime(CurrentCalendar.GetYear(DateTime),
                CurrentCalendar.GetMonth(newDateTime),
                GetRoundDay(CurrentCalendar.GetYear(DateTime),
                    CurrentCalendar.GetMonth(newDateTime),
                    CurrentCalendar.GetDayOfMonth(DateTime)), DateTime.Hour, DateTime.Minute, DateTime.Second,
                DateTime.Millisecond);
        }

        protected override void OnYearCellButtonClick(Button button)
        {
            SetYear((DateTime) GetDateTime(button));
            SetNewContent(DateTime, DateEditCalendarState.Year, YearInfoTemplate, DateEditCalendarTransferType.ZoomIn);
        }

        protected override void OnDateTimeChanged()
        {
            base.OnDateTimeChanged();
            if (OnMyDateTimeChanged != null)
                OnMyDateTimeChanged.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnMyDateTimeChanged;
    }
}
