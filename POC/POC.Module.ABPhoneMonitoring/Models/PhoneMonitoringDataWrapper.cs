using POL.Lib.Interfaces;
using POL.WPF.DXControls.MVVM;
using System;
using System.Windows;
using System.Windows.Media;

namespace POC.Module.ABPhoneMonitoring.Models
{
    public class PhoneMonitoringDataWrapper : NotifyObjectBase
    {
        private DateTime RingDate = DateTime.MinValue;
        private Brush LastBrush = Brushes.LightGray;
        private bool _ForceShow = false;
        private TimeSpan _Duration = TimeSpan.FromSeconds(0);

        #region Data
        private PhoneMonitoringData _Data;
        public PhoneMonitoringData Data
        {
            get { return _Data; }
            set { _Data = value; }
        }
        #endregion


        public EnumCallSystemLineStatus Status
        {
            get { return Data.Status; }
        }

        public string OutContactTitle
        {
            get { return Data.OutContactTitle; }
        }
        public string OutPhoneNumber
        {
            get { return Data.OutPhoneNumber; }
        }
        public int OutPhoneCityCode
        {
            get { return Data.OutPhoneCityCode; }
        }
        public int OutPhoneCountryCode
        {
            get { return Data.OutPhoneCountryCode; }
        }
        public string OutPhoneCityName
        {
            get { return Data.OutPhoneCityName; }
        }
        public string OutPhoneCountryName
        {
            get { return Data.OutPhoneCountryName; }
        }
        public DateTime OutDateTime
        {
            get { return Data.OutDateTime; }
        }
        public TimeSpan OutDuration
        {
            get { return Data.OutDuration; }
        }
        public bool OutTalking
        {
            get { return Data.OutTalking; }
        }
        public string OutExt
        {
            get { return Data.OutExt; }
        }
        public string OutDialed
        {
            get { return Data.OutDialed; }
        }

        public string InContactTitle
        {
            get { return Data.InContactTitle; }
        }
        public string InPhoneNumber
        {
            get { return Data.InPhoneNumber; }
        }
        public int InPhoneCityCode
        {
            get { return Data.InPhoneCityCode; }
        }
        public int InPhoneCountryCode
        {
            get { return Data.InPhoneCountryCode; }
        }
        public string InPhoneCityName
        {
            get { return Data.InPhoneCityName; }
        }
        public string InPhoneCountryName
        {
            get { return Data.InPhoneCountryName; }
        }
        public DateTime InDateTime
        {
            get { return Data.InDateTime; }
        }
        public TimeSpan InDuration
        {
            get { return Data.InDuration; }
        }
        public bool InTalking
        {
            get { return Data.InTalking; }
        }
        public string InExt
        {
            get { return Data.InExt; }
        }
        public DateTime InLastRingDate
        {
            get { return Data.InLastRingDate; }
        }





        public void SetForceShow(bool force)
        {
            _ForceShow = force;
            RaisePropertyChanged("ControlVisibility");
        }

        #region ControlVisibility

        public Visibility ControlVisibility
        {
            get
            {
                return Visibility.Collapsed;
            }
        }

        #endregion

        #region TextLineDuration
        public string TextLineDuration
        {
            get { return string.Format("{0:00}:{1:00}:{2:00}", _Duration.Hours, _Duration.Minutes, _Duration.Seconds); }
        }
        #endregion

        public Brush BackgroundLineDuration
        {
            get { return TextLineDuration == "00:00:00" ? Brushes.Transparent : Brushes.Tomato; }
        }


        public string TextCallInDuration
        {
            get
            {
                return string.Format("{0:00}:{1:00}:{2:00}", InDuration.TotalHours, InDuration.Minutes,
                                     InDuration.Seconds);
            }
        }
        public string TextCallInPhone
        {
            get { return (InPhoneCityCode > 0 ? InPhoneCityCode + ", " : string.Empty) + InPhoneNumber + (string.IsNullOrWhiteSpace(InExt) ? "" : "(" + InExt + ")"); }
        }
        public string TextCallInTime
        {
            get { return string.Format("{0:00}:{1:00}:{2:00}", InDateTime.Hour, InDateTime.Minute, InDateTime.Second); }
        }
        public string TextCallInTitle
        {
            get { return InContactTitle; }
        }

        public string TextCallOutDuration
        {
            get
            {
                return string.Format("{0:00}:{1:00}:{2:00}", OutDuration.TotalHours, OutDuration.Minutes,
                                     OutDuration.Seconds);
            }
        }
        public string TextCallOutPhone
        {
            get { return (OutPhoneCityCode > 0 ? OutPhoneCityCode + ", " : string.Empty) + OutPhoneNumber + (string.IsNullOrWhiteSpace(OutExt) ? "" : "(" + OutExt + ")"); }
        }
        public string TextCallOutTime
        {
            get { return string.Format("{0:00}:{1:00}:{2:00}", OutDateTime.Hour, OutDateTime.Minute, OutDateTime.Second); }
        }
        public string TextCallOutTitle
        {
            get { return OutContactTitle; }
        }

        public string TextExtraTitle
        {
            get { return Data.InExt; }
        }

        public Brush BackgroundHeader
        {
            get
            {
                if (Status == EnumCallSystemLineStatus.Ring && (DateTime.Now - RingDate).TotalSeconds > 4)
                {
                    LastBrush = Brushes.MediumPurple;
                    return LastBrush;
                }
                switch (Status)
                {
                    case EnumCallSystemLineStatus.Unkown:
                        LastBrush = Brushes.LightGray;
                        break;
                    case EnumCallSystemLineStatus.Ring:
                        LastBrush = Brushes.Orange;
                        break;
                    case EnumCallSystemLineStatus.HookOn:
                        LastBrush = Brushes.LawnGreen;
                        break;
                    case EnumCallSystemLineStatus.HookOff:
                        LastBrush = Brushes.LightSkyBlue;
                        break;
                    case EnumCallSystemLineStatus.CallerID:
                        break;
                    case EnumCallSystemLineStatus.Dialed:
                        break;
                }
                return LastBrush;
            }
        }


        internal void SetData(PhoneMonitoringData data)
        {
            if (Data.Status != EnumCallSystemLineStatus.Ring && data.Status == EnumCallSystemLineStatus.Ring)
                RingDate = DateTime.Now;

            if (Data.Status == EnumCallSystemLineStatus.HookOn && data.Status == EnumCallSystemLineStatus.Trans)
                _Duration = TimeSpan.FromSeconds(0);

            Data = data;

            if (data.Status == EnumCallSystemLineStatus.HookOn)
                _Duration = _Duration.Add(TimeSpan.FromSeconds(1));
            else if (data.Status == EnumCallSystemLineStatus.Trans)
            {
                _Duration = _Duration.Add(TimeSpan.FromSeconds(1));
            }
            else
            {
                _Duration = TimeSpan.FromSeconds(0);
            }

            RaisePropertyChanged("ControlVisibility");
            RaisePropertyChanged("TextLineDuration");
            RaisePropertyChanged("BackgroundLineDuration");

            RaisePropertyChanged("TextCallInDuration");
            RaisePropertyChanged("TextCallInPhone");
            RaisePropertyChanged("TextCallInTime");
            RaisePropertyChanged("TextCallInTitle");

            RaisePropertyChanged("TextCallOutDuration");
            RaisePropertyChanged("TextCallOutPhone");
            RaisePropertyChanged("TextCallOutTime");
            RaisePropertyChanged("TextCallOutTitle");

            RaisePropertyChanged("BackgroundHeader");

            RaisePropertyChanged("TextExtraTitle");

            Console.WriteLine(data.InExt);
        }
    }
}
