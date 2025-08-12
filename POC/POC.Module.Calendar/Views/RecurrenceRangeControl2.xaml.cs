using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.XtraScheduler;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;

namespace POC.Module.Calendar.Views
{
    public partial class RecurrenceRangeControl2 : UserControl, INotifyPropertyChanged
    {
        public RecurrenceRangeControl2()
        {
            InitializeComponent();
            MainElement.DataContext = this;

        }

        static RecurrenceRangeControl2()
        {
            RecurrenceInfoProperty = CreatePatternRecurrenceInfoProperty();
            TimeZoneHelperProperty = CreateTimeZoneHelperProperty();
            PatternProperty = CreatePatternProperty();
            IsReadOnlyProperty = DependencyPropertyHelper.RegisterProperty<RecurrenceRangeControl2, bool>("IsReadOnly", false);
        }
        private static DependencyProperty CreatePatternProperty()
        {
            return DependencyPropertyHelper.RegisterProperty<RecurrenceRangeControl2, Appointment>("Pattern", null);
        }

        private static DependencyProperty CreatePatternRecurrenceInfoProperty()
        {
            return DependencyPropertyHelper.RegisterProperty<RecurrenceRangeControl2, RecurrenceInfo>("RecurrenceInfo", null, delegate(RecurrenceRangeControl2 d, DependencyPropertyChangedEventArgs<RecurrenceInfo> e)
            {
                d.OnPatternRecurrenceInfoChanged(e.OldValue, e.NewValue);
            }, null);
        }

        private static DependencyProperty CreateTimeZoneHelperProperty()
        {
            return DependencyPropertyHelper.RegisterProperty<RecurrenceRangeControl2, TimeZoneHelper>("TimeZoneHelper", null, delegate(RecurrenceRangeControl2 d, DependencyPropertyChangedEventArgs<TimeZoneHelper> e)
            {
                d.OnTimeZoneHelperChanged(e.OldValue, e.NewValue);
            }, null);
        }

        private void EndByDateRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.RecurrenceInfo.Range = RecurrenceRange.EndByDate;
        }

        protected internal virtual DateTime FromClientTime(DateTime date)
        {
            if (this.TimeZoneHelper != null)
            {
                return this.TimeZoneHelper.FromClientTime(date);
            }
            return date;
        }

        protected virtual IList GetRecurrenceRanges()
        {
            return new NamedElementList { new NamedElement((RecurrenceRange)0, SchedulerControlLocalizer.GetString((SchedulerControlStringId)0x2d), this), new NamedElement((RecurrenceRange)1, SchedulerControlLocalizer.GetString((SchedulerControlStringId)0x2e), this), new NamedElement((RecurrenceRange)2, SchedulerControlLocalizer.GetString((SchedulerControlStringId)0x2f), this) };
        }

        private void NoEndDateRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.RecurrenceInfo.Range = RecurrenceRange.NoEndDate;
        }

        private void OccurenceCountRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.RecurrenceInfo.Range = RecurrenceRange.OccurrenceCount;
        }

        private void OnEndByDateEditGotFocus(object sender, RoutedEventArgs e)
        {
            if (this.RecurrenceInfo != null)
            {
                this.RecurrenceInfo.Range = RecurrenceRange.EndByDate;
                if (!this.IsReadOnly)
                {
                    this.EndByDateRadioButton.IsChecked = true;
                }
            }
        }

        private void OnOccurrenceCountSpinEditGotFocus(object sender, RoutedEventArgs e)
        {
            if (this.RecurrenceInfo != null)
            {
                this.RecurrenceInfo.Range = RecurrenceRange.OccurrenceCount;
                if (!this.IsReadOnly)
                {
                    this.OccurenceCountRadioButton.IsChecked = true;
                }
            }
        }

        private void OnPatternRecurrenceInfoChanged(RecurrenceInfo oldValue, RecurrenceInfo newValue)
        {
            if (oldValue != null)
            {
                oldValue.PropertyChanged -= new PropertyChangedEventHandler(this.OnPatternRecurrenceInfoPropertyChanged);
            }
            if (newValue != null)
            {
                newValue.PropertyChanged += new PropertyChangedEventHandler(this.OnPatternRecurrenceInfoPropertyChanged);
                this.SetRecurrenceRange(newValue.Range);
            }
            this.UpdateRangeControl();
        }

        private void OnPatternRecurrenceInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateRange(this.RecurrenceInfo, this.RecurrenceInfo.Start, this.RecurrenceInfo.End, this.RecurrenceInfo.Range, this.RecurrenceInfo.OccurrenceCount, this.Pattern);
            this.UpdateRangeControl();
        }

        private void UpdateRange(RecurrenceInfo info, DateTime start, DateTime end, RecurrenceRange rangeType, int occurrencesCount, Appointment pattern)
        {
            info.BeginUpdate();
            try
            {
                info.Start = start;
                info.Range = rangeType;
                if (rangeType == RecurrenceRange.EndByDate)
                {
                    info.End = end;
                    info.OccurrenceCount = CalcOccurrenceCountInRangeCore(info, pattern, 0x2710);
                }
                else
                {
                    info.OccurrenceCount = occurrencesCount;
                    OccurrenceCalculator calculator = OccurrenceCalculator.CreateInstance(CreateValidRecurrenceInfoCopy(info));
                    try
                    {
                        info.End = calculator.CalcOccurrenceStartTime(occurrencesCount - 1);
                    }
                    catch
                    {
                        info.Duration = (TimeSpan)(DateTime.MaxValue - info.Start);
                    }
                }
            }
            finally
            {
                info.EndUpdate();
            }
        }
        private int CalcOccurrenceCountInRangeCore(RecurrenceInfo info, Appointment pattern, int maxCount)
        {
            var calculator = OccurrenceCalculator.CreateInstance(CreateValidRecurrenceInfoCopy(info));
            var interval = new TimeInterval(info.Start,GetActualRecurrenceEnd(calculator, info));
            return Math.Max(1, CalcOccurrenceCountCore(calculator, info, interval, pattern, maxCount));
        }
        private RecurrenceInfo CreateValidRecurrenceInfoCopy(RecurrenceInfo rinfo)
        {
            RecurrenceInfo info = new RecurrenceInfo();
            Assign(rinfo, info);
            if (info.WeekDays == 0)
            {
                info.WeekDays = WeekDays.Sunday;
            }
            return info;
        }

        protected internal virtual DateTime GetActualRecurrenceEnd(OccurrenceCalculator calc, RecurrenceInfo info)
        {
            return (info.End.Date + (new TimeSpan(1, 0, 0, 0)));
        }

        protected internal int CalcOccurrenceCountCore(OccurrenceCalculator calc, RecurrenceInfo info, TimeInterval interval, Appointment pattern, int maxValue)
        {
            DateTime time;
            int index = calc.FindFirstOccurrenceIndex(interval, pattern);
            if (index < 0)
            {
                return 0;
            }
        Label_000F:
            time = calc.CalcOccurrenceStartTime(index);
            if (! IsOccurrenceIncorrect(calc, info, interval, time, pattern.Duration, index))
            {
                index++;
                if (index >= maxValue)
                {
                    return index;
                }
                goto Label_000F;
            }
            return index;
        }

        internal bool IsOccurrenceIncorrect(OccurrenceCalculator calc, RecurrenceInfo info, TimeInterval interval, DateTime occurrenceStart, TimeSpan occurrenceDuration, int occurrenceIndex)
        {
            if ((info.Range != RecurrenceRange.EndByDate) || (occurrenceStart < GetActualRecurrenceEnd(calc, info)))
            {
                if ((info.Range == RecurrenceRange.OccurrenceCount) && (occurrenceIndex >= info.OccurrenceCount))
                {
                    return true;
                }
                if (occurrenceDuration == TimeSpan.Zero)
                {
                    occurrenceDuration = TimeSpan.FromTicks(1L);
                }
                if (interval.Duration <= TimeSpan.Zero)
                {
                    return (occurrenceStart > interval.End);
                }
                if (occurrenceStart < interval.End)
                {
                    return ((occurrenceStart + occurrenceDuration) <= interval.Start);
                }
            }
            return true;
        }

        protected internal void Assign(RecurrenceInfo src, RecurrenceInfo info)
        {
            info.BeginUpdate();
            try
            {
                info.DayNumber = src.DayNumber;
                info.WeekOfMonth = src.WeekOfMonth;
                info.Periodicity = src.Periodicity;
                info.Month = src.Month;
                info.OccurrenceCount = src.OccurrenceCount;
                info.Range = src.Range;
                info.Type = src.Type;
                info.WeekDays = src.WeekDays;
                info.AllDay = src.AllDay;
                info.Start = src.Start;
                info.Duration = src.Duration;
            }
            finally
            {
                info.EndUpdate();
            }
        }















        private void OnTimeZoneHelperChanged(TimeZoneHelper oldTimeZoneHelper, TimeZoneHelper newTimeZoneHelper)
        {
            this.UpdateRangeControl();
        }

        protected virtual void RaiseOnPropertyChanged(string propertyName)
        {
            if (this.onPropertyChanged != null)
            {
                this.onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected internal virtual void SetRecurrenceRange(RecurrenceRange type)
        {
            switch (type)
            {
                case RecurrenceRange.NoEndDate:
                    this.NoEndDateRadioButton.IsChecked = true;
                    return;

                case RecurrenceRange.OccurrenceCount:
                    this.OccurenceCountRadioButton.IsChecked = true;
                    return;
            }
            this.EndByDateRadioButton.IsChecked = true;
        }

        protected internal virtual DateTime ToClientTime(DateTime date)
        {
            if (this.TimeZoneHelper != null)
            {
                return this.TimeZoneHelper.ToClientTime(date);
            }
            return date;
        }

        protected virtual void UpdateRangeControl()
        {
            this.RaiseOnPropertyChanged("LocalStart");
            this.RaiseOnPropertyChanged("LocalEnd");
        }










        public bool IsReadOnly
        {
            get
            {
                return (bool)base.GetValue(IsReadOnlyProperty);
            }
            set
            {
                base.SetValue(IsReadOnlyProperty, value);
            }
        }

        public DateTime LocalEnd
        {
            get
            {
                var d = RecurrenceInfo == null ? DateTime.MinValue : ToClientTime(RecurrenceInfo.End);

                return d;
            }
            set
            {
                if (RecurrenceInfo != null)
                {
                    try
                    {
                        RecurrenceInfo.End = FromClientTime(value);
                    }catch
                    {
                        RecurrenceInfo.End = DateTime.Now.AddYears(1);
                    }
                }
            }
        }
        public DateTime LocalStart
        {
            get
            {
                if (this.RecurrenceInfo == null)
                {
                    return DateTime.MinValue;
                }
                return this.ToClientTime(this.RecurrenceInfo.Start);
            }
        }

        public Appointment Pattern
        {
            get
            {
                return (Appointment)base.GetValue(PatternProperty);
            }
            set
            {
                base.SetValue(PatternProperty, value);
            }
        }
        public RecurrenceInfo RecurrenceInfo
        {
            get
            {
                return (RecurrenceInfo)base.GetValue(RecurrenceInfoProperty);
            }
            set
            {
                base.SetValue(RecurrenceInfoProperty, value);
            }
        }

        public IList RecurrenceRanges
        {
            get
            {
                return this.GetRecurrenceRanges();
            }
        }

        public TimeZoneHelper TimeZoneHelper
        {
            get
            {
                return (TimeZoneHelper)base.GetValue(TimeZoneHelperProperty);
            }
            set
            {
                base.SetValue(TimeZoneHelperProperty, value);
            }
        }





        public static readonly DependencyProperty IsReadOnlyProperty;





        private PropertyChangedEventHandler onPropertyChanged;
        public static readonly DependencyProperty PatternProperty;










        public static readonly DependencyProperty RecurrenceInfoProperty;
        public static readonly DependencyProperty TimeZoneHelperProperty;


















































































        public event PropertyChangedEventHandler PropertyChanged;
    }
}
