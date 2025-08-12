using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POL.WPF.DXControls.POLCalendarGadget
{
    public partial class POLCalendarGadget
    {
        public POLCalendarGadget()
        {
            InitializeComponent();
            UpdateValue();
        }


        private void UpdateValue()
        {
            var calendar = HelperLocalize.GetCalendar(CalendarType);
            var hijriCalendar = calendar as HijriCalendar;
            if (hijriCalendar != null)
                hijriCalendar.HijriAdjustment = HijriOffset;

            var dd = calendar.GetDayOfMonth(DateEditValue);
            var mm = calendar.GetMonth(DateEditValue);
            var yy = calendar.GetYear(DateEditValue);

            var dddd = HelperLocalize.GetDayName(CalendarType, DateEditValue.DayOfWeek);
            var mmmm = HelperLocalize.GetMonthName(CalendarType, mm);

            tbDayNumber.Text = dd.ToString(CultureInfo.InvariantCulture);
            tbMonth.Text = string.Format("{0} ({1})", mmmm, mm);
            tbDayOfWeek.Text = dddd;
            tbYear.Text = yy.ToString(CultureInfo.InvariantCulture);

            tbDayNumber.FontSize = tbMonth.FontSize*5;


            if (CalendarType == EnumCalendarType.Gregorian)
            {
                tbMonth.FlowDirection = FlowDirection.LeftToRight;
            }
        }

        #region DateEditValue Dependancy Property

        public static readonly DependencyProperty DateEditValueProperty = DependencyProperty.Register(
            "DateEditValue",
            typeof (DateTime),
            typeof (POLCalendarGadget),
            new UIPropertyMetadata(DateTime.Now, DateEditValue_CallBack));

        public DateTime DateEditValue
        {
            get { return (DateTime) GetValue(DateEditValueProperty); }
            set { SetValue(DateEditValueProperty, value); }
        }

        private static void DateEditValue_CallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var self = (POLCalendarGadget) d;
                if (self == null) return;
                if (e.NewValue == e.OldValue) return;
                self.UpdateValue();
            }
            catch
            {
            }
        }

        #endregion

        #region CalendarType Dependancy Property

        public static readonly DependencyProperty CalendarTypeProperty = DependencyProperty.Register(
            "CalendarType",
            typeof (EnumCalendarType),
            typeof (POLCalendarGadget),
            new UIPropertyMetadata(EnumCalendarType.Shamsi, CalendarType_CallBack));

        public EnumCalendarType CalendarType
        {
            get { return (EnumCalendarType) GetValue(CalendarTypeProperty); }
            set { SetValue(CalendarTypeProperty, value); }
        }

        private static void CalendarType_CallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var self = (POLCalendarGadget) d;
                if (self == null) return;
                if (e.NewValue == e.OldValue) return;
                self.UpdateValue();
            }
            catch
            {
            }
        }

        #endregion

        #region HijriOffset Dependancy Property

        public int HijriOffset
        {
            get { return (int) GetValue(HijriOffsetProperty); }
            set { SetValue(HijriOffsetProperty, value); }
        }

        public static readonly DependencyProperty HijriOffsetProperty =
            DependencyProperty.Register("HijriOffset", typeof (int), typeof (POLCalendarGadget),
                new PropertyMetadata(0, HijriOffset_CallBack));

        private static void HijriOffset_CallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var self = (POLCalendarGadget) d;
                if (self == null) return;
                if (e.NewValue == e.OldValue) return;
                self.UpdateValue();
            }
            catch
            {
            }
        }

        #endregion

        #region FillBrush Dependancy Property

        public static readonly DependencyProperty FillBrushProperty = DependencyProperty.Register(
            "FillBrush",
            typeof (Brush),
            typeof (POLCalendarGadget),
            new UIPropertyMetadata(Brushes.OrangeRed, FillBrush_CallBack));

        public Brush FillBrush
        {
            get { return (Brush) GetValue(FillBrushProperty); }
            set { SetValue(FillBrushProperty, value); }
        }

        private static void FillBrush_CallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var self = (POLCalendarGadget) d;
                if (self == null) return;
                if (e.NewValue == e.OldValue) return;
                self.bBorder.Background = (Brush) e.NewValue;
            }
            catch
            {
            }
        }

        #endregion
    }
}
