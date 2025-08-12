using System;
using System.Collections.ObjectModel;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using Microsoft.Practices.ServiceLocation;
using POL.WPF.DXControls.MVVM;
using System.Windows;

namespace POC.Module.ABCalendar.Models
{
    public class MSettingPrayTime : NotifyObjectBase
    {
        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }

        public MSettingPrayTime(object mainView)
        {
            MainView = mainView;
            InitCommands();
            InitDynamics();
        }

        private void InitDynamics()
        {
            DynamicOwner = Window.GetWindow(MainView as UIElement);
        }

        private void InitCommands()
        {
            CommandSelectLatLong = new RelayCommand(() =>
            {
                var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                var loc = v.ShowSelectPointOnMap(DynamicOwner, new MapLocationItem { Lat = Latitude, Lon = Longitude, ZoomLevel = 8 });
                if (loc == null) return;
                Latitude = loc.Lat;
                Longitude = loc.Lon;
            });
        }

        public ReadOnlyCollection<TimeZoneInfo> TimeZoneList
        {
            get { return TimeZoneInfo.GetSystemTimeZones(); }
        }
        public TimeZoneInfo SelectedTimeZone
        {
            get
            {
                try
                {
                    var id = HelperSettingsClient.PrayTimeTimeZoneID;
                    return TimeZoneInfo.FindSystemTimeZoneById(id);
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                HelperSettingsClient.PrayTimeTimeZoneID = value.Id;
                RaisePropertyChanged("SelectedTimeZone");
            }
        }

        public double Latitude
        {
            get { return (double)HelperSettingsClient.PrayTimeLatitude / 1000; }
            set
            {
                HelperSettingsClient.PrayTimeLatitude = (int)(value * 1000);
                RaisePropertyChanged("Latitude");
            }
        }
        public double Longitude
        {
            get { return (double)HelperSettingsClient.PrayTimeLongitude / 1000; }
            set
            {
                HelperSettingsClient.PrayTimeLongitude = (int)(value * 1000);
                RaisePropertyChanged("Longitude");
            }
        }
        public int SelectedCalculationMethod
        {
            get { return HelperSettingsClient.PrayTimeCalculationMethod; }
            set
            {
                HelperSettingsClient.PrayTimeCalculationMethod = value;
                RaisePropertyChanged("Longitude");
            }
        }
        public int SelectedJuristicMethod
        {
            get { return HelperSettingsClient.PrayTimeJuristicMethod; }
            set
            {
                HelperSettingsClient.PrayTimeJuristicMethod = value;
                RaisePropertyChanged("Longitude");
            }
        }
        public int SelectedHigherLatitude
        {
            get { return HelperSettingsClient.PrayTimeHigherLatitude; }
            set
            {
                HelperSettingsClient.PrayTimeHigherLatitude = value;
                RaisePropertyChanged("Longitude");
            }
        }


        public RelayCommand CommandSelectLatLong { set; get; }
    }
}
