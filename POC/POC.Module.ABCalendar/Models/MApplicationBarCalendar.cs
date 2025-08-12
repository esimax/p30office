using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;

namespace POC.Module.ABCalendar.Models
{
    public class MApplicationBarCalendar : IDisposable, INotifyPropertyChanged
    {
        private static IMembership AMembership { get; set; }
        private static IDatabase ADatabase { get; set; }
        private static IPOCContactModule AContactModule { get; set; }


        public MApplicationBarCalendar(object mainView)
        {
            if (AMembership == null)
            {
                AMembership = ServiceLocator.Current.GetInstance<IMembership>();
                AMembership.OnMembershipStatusChanged += 
                    (s, e) =>
                    {
                        OnPropertyChanged("CalendarEventList");

                        switch (e.Status)
                        {
                            case EnumMembershipStatus.InvalidNetwork:
                                break;
                            case EnumMembershipStatus.AccessDenide:
                                break;
                            case EnumMembershipStatus.BeforLogin:
                                break;
                            case EnumMembershipStatus.AfterLogin:
                                break;
                            case EnumMembershipStatus.BeforeLogout:
                                break;
                            case EnumMembershipStatus.AfterLogout:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    };
            }
            if (ADatabase == null)
                ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            if (AContactModule == null)
                AContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();

            HijriOffset = HelperSettingsClient.HijriCalendarOffset;
            InitCommands();
            SelectedDate = DateTime.Now.Date;

            CalculatePrayTime();

            OneSecondTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 1000) };
            OneSecondTimer.Tick += (s, e) => OnPropertyChanged("CurrentTime");
            OneSecondTimer.Start();
        }





        private DispatcherTimer OneSecondTimer { get; set; }
        public string CurrentTime
        {
            get { return string.Format("{0}:{1}:{2}", DateTime.Now.Hour.ToString().PadLeft(2, '0'), DateTime.Now.Minute.ToString().PadLeft(2, '0'), DateTime.Now.Second.ToString().PadLeft(2, '0')); }
        }


        #region SelectedDate
        private DateTime _SelectedDate;
        public DateTime SelectedDate
        {
            get { return _SelectedDate; }
            set
            {
                _SelectedDate = value;
                OnPropertyChanged("SelectedDate");
                OnPropertyChanged("DayOffsetText");
                OnPropertyChanged("DayOffsetBrush");
                CalculatePrayTime();

                OnPropertyChanged("CalendarEventList");

                AContactModule.SelectedDate = value;
                AContactModule.RaiseOnSelectedDateChanged();
            }
        }
        #endregion

        #region DayOffsetText
        public string DayOffsetText
        {
            get
            {
                var delta = DateTime.Now.Date - SelectedDate;
                return ((int)delta.TotalDays) == 0 ? "امروز" : string.Format("{0} روز {1}", (int)Math.Abs(delta.TotalDays), (delta.TotalDays < 0 ? "بعد" : "قبل"));
            }
        }
        #endregion

        #region DayOffsetBrush
        public Brush DayOffsetBrush
        {
            get
            {
                var delta = DateTime.Now.Date - SelectedDate;
                return ((int)delta.TotalDays) == 0 ? Brushes.Transparent : Brushes.Yellow;
            }
        }
        #endregion

        public int HijriOffset { get; set; }

        public string PrayTimeFajr { get; private set; }
        public string PrayTimeSunrise { get; private set; }
        public string PrayTimeDhuhr { get; private set; }
        public string PrayTimeAsr { get; private set; }
        public string PrayTimeSunset { get; private set; }
        public string PrayTimeMaghrib { get; private set; }
        public string PrayTimeIsha { get; private set; }



        public List<string> CalendarEventList
        {
            get
            {
                if (AMembership == null) return null;
                if (!AMembership.IsAuthorized) return null;
                try
                {
                    var xpc = POL.DB.General.DBGLCalendarEvent.GetByDateTime(ADatabase.Dxs, SelectedDate, HijriOffset);
                    var q = from n in xpc select n.EventTitle;
                    return q.ToList();
                }
                catch
                {
                    return null;
                }
            }
        }


        #region [METHODS]
        private void InitCommands()
        {
            CommandPrevMonth = new RelayCommand(PrevMonth, () => true);
            CommandPrevWeek = new RelayCommand(PrevWeek, () => true);
            CommandPrevDay = new RelayCommand(PrevDay, () => true);
            CommandToday = new RelayCommand(Today, () => true);
            CommandNextDay = new RelayCommand(NextDay, () => true);
            CommandNextWeek = new RelayCommand(NextWeek, () => true);
            CommandNextMonth = new RelayCommand(NextMonth, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp01!="");
        }

        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp01);
        }

        private void PrevMonth()
        {
            SelectedDate = SelectedDate.AddMonths(-1);
        }
        private void PrevWeek()
        {
            SelectedDate = SelectedDate.AddDays(-7);
        }
        private void PrevDay()
        {
            SelectedDate = SelectedDate.AddDays(-1);
        }
        private void Today()
        {
            SelectedDate = DateTime.Now.Date;
        }
        private void NextDay()
        {
            SelectedDate = SelectedDate.AddDays(1);
        }
        private void NextWeek()
        {
            SelectedDate = SelectedDate.AddDays(7);
        }
        private void NextMonth()
        {
            SelectedDate = SelectedDate.AddMonths(1);
        }

        private void CalculatePrayTime()
        {
            try
            {
                var pt = new PrayTime();
                var lo = (double)HelperSettingsClient.PrayTimeLongitude / 1000;
                var la = (double)HelperSettingsClient.PrayTimeLatitude / 1000;
                var y = SelectedDate.Year;
                var m = SelectedDate.Month;
                var d = SelectedDate.Day;

                var tz =
                    TimeZoneInfo.FindSystemTimeZoneById(HelperSettingsClient.PrayTimeTimeZoneID)
                        .GetUtcOffset(SelectedDate.Date).TotalMinutes / 60;

                var calc = HelperSettingsClient.PrayTimeCalculationMethod;
                if (calc == PrayTime.Custom) calc = PrayTime.Tehran;

                pt.setCalcMethod(calc);
                pt.setAsrMethod(HelperSettingsClient.PrayTimeJuristicMethod);
                pt.setHighLatsMethod(HelperSettingsClient.PrayTimeHigherLatitude);
                var s = pt.getDatePrayerTimes(y, m, d, la, lo, tz);
                PrayTimeFajr = s[0];
                PrayTimeSunrise = s[1];
                PrayTimeDhuhr = s[2];
                PrayTimeAsr = s[3];
                PrayTimeSunset = s[4];
                PrayTimeMaghrib = s[5];
                PrayTimeIsha = s[6];
            }
            catch
            {
                const string err = "خطا در تنظیمات";
                PrayTimeFajr = err;
                PrayTimeSunrise = err;
                PrayTimeDhuhr = err;
                PrayTimeAsr = err;
                PrayTimeSunset = err;
                PrayTimeMaghrib = err;
                PrayTimeIsha = err;
            }

            OnPropertyChanged("PrayTimeFajr");
            OnPropertyChanged("PrayTimeSunrise");
            OnPropertyChanged("PrayTimeDhuhr");
            OnPropertyChanged("PrayTimeAsr");
            OnPropertyChanged("PrayTimeSunset");
            OnPropertyChanged("PrayTimeMaghrib");
            OnPropertyChanged("PrayTimeIsha");
        }
        #endregion



        #region [COMMANDS]
        public RelayCommand CommandPrevMonth { get; set; }
        public RelayCommand CommandPrevWeek { get; set; }
        public RelayCommand CommandPrevDay { get; set; }
        public RelayCommand CommandToday { get; set; }
        public RelayCommand CommandNextDay { get; set; }
        public RelayCommand CommandNextWeek { get; set; }
        public RelayCommand CommandNextMonth { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (OneSecondTimer != null)
                OneSecondTimer.Stop();
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
