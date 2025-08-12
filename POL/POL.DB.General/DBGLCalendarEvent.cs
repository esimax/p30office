using System;
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Interfaces;

namespace POL.DB.General
{
    [OptimisticLocking(false), DeferredDeletion(false)]
    public class DBGLCalendarEvent : XPInt32Object
    {
        #region Design
        public DBGLCalendarEvent(Session session) : base(session)
        {
        }
        #endregion

        public static XPCollection<DBGLCalendarEvent> GetByDateTime(Session dxs, DateTime d, int hijriOffset)
        {
            var s_pc = new PersianCalendar();
            var s_m = s_pc.GetMonth(d);
            var s_d = s_pc.GetDayOfMonth(d);

            var h_pc = new HijriCalendar {HijriAdjustment = hijriOffset};
            var h_m = h_pc.GetMonth(d);
            var h_d = h_pc.GetDayOfMonth(d);

            var go = new GroupOperator(GroupOperatorType.Or);
            go.Operands.Add(new GroupOperator(new BinaryOperator("CalendarType", (int) EnumCalendarType.Shamsi),
                new BinaryOperator("MonthNumber", s_m),
                new BinaryOperator("DayNumber", s_d)));

            go.Operands.Add(new GroupOperator(new BinaryOperator("CalendarType", (int) EnumCalendarType.Hijri),
                new BinaryOperator("MonthNumber", h_m),
                new BinaryOperator("DayNumber", h_d)));

            go.Operands.Add(new GroupOperator(new BinaryOperator("CalendarType", (int) EnumCalendarType.Gregorian),
                new BinaryOperator("MonthNumber", d.Month),
                new BinaryOperator("DayNumber", d.Day)));

            var rv = new XPCollection<DBGLCalendarEvent>(dxs, go);
            return rv;
        }

        #region CalendarType

        private EnumCalendarType _CalendarType;

        public EnumCalendarType CalendarType
        {
            get { return _CalendarType; }
            set { SetPropertyValue("CalendarType", ref _CalendarType, value); }
        }

        #endregion

        #region MonthNumber

        private int _MonthNumber;

        public int MonthNumber
        {
            get { return _MonthNumber; }
            set { SetPropertyValue("MonthNumber", ref _MonthNumber, value); }
        }

        #endregion

        #region DayNumber

        private int _DayNumber;

        public int DayNumber
        {
            get { return _DayNumber; }
            set { SetPropertyValue("DayNumber", ref _DayNumber, value); }
        }

        #endregion

        #region EventTitle

        private string _EventTitle;

        [PersianString, Size(1024)]
        public string EventTitle
        {
            get { return _EventTitle; }
            set { SetPropertyValue("EventTitle", ref _EventTitle, value); }
        }

        #endregion

        #region IsDayOff

        private bool _IsDayOff;

        public bool IsDayOff
        {
            get { return _IsDayOff; }
            set { SetPropertyValue("IsDayOff", ref _IsDayOff, value); }
        }

        #endregion
    }
}
