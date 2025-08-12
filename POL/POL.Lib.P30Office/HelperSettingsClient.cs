using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;

namespace POL.Lib.XOffice
{
    public class HelperSettingsClient
    {
        #region AutoStart

        public static int AutoStartDelay
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntAutoStartDelay); }
            set { Settings.SetValue(EnumPOCAppSettings.IntAutoStartDelay, value); }
        }

        #endregion

        public static string ABCalendarLayout
        {
            get { return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringABCalendarLayout); }
            set { Settings.SetValue(EnumPOCAppSettings.StringABCalendarLayout, value); }
        }

        public static bool ABPhoneMonShowAll
        {
            get { return Settings.GetValue<bool, EnumPOCAppSettings>(EnumPOCAppSettings.BoolABPhoneMonShowAll); }
            set { Settings.SetValue(EnumPOCAppSettings.BoolABPhoneMonShowAll, value); }
        }

        public static bool ABPhoneMonShowIn
        {
            get { return Settings.GetValue<bool, EnumPOCAppSettings>(EnumPOCAppSettings.BoolABPhoneMonShowIn); }
            set { Settings.SetValue(EnumPOCAppSettings.BoolABPhoneMonShowIn, value); }
        }

        public static bool ABPhoneMonShowOut
        {
            get { return Settings.GetValue<bool, EnumPOCAppSettings>(EnumPOCAppSettings.BoolABPhoneMonShowOut); }
            set { Settings.SetValue(EnumPOCAppSettings.BoolABPhoneMonShowOut, value); }
        }

        public static bool ABPhoneMonShowExtra
        {
            get { return Settings.GetValue<bool, EnumPOCAppSettings>(EnumPOCAppSettings.BoolABPhoneMonShowExtra); }
            set { Settings.SetValue(EnumPOCAppSettings.BoolABPhoneMonShowExtra, value); }
        }

        #region Phone Contact Module

        public static bool PhoneViewIsFull
        {
            get { return Settings.GetValue<bool, EnumPOCAppSettings>(EnumPOCAppSettings.BoolPhoneViewIsFull); }
            set { Settings.SetValue(EnumPOCAppSettings.BoolPhoneViewIsFull, value); }
        }

        #endregion

        #region Address Contact Module

        public static bool AddressViewIsFull
        {
            get { return Settings.GetValue<bool, EnumPOCAppSettings>(EnumPOCAppSettings.BoolAddressViewIsFull); }
            set { Settings.SetValue(EnumPOCAppSettings.BoolAddressViewIsFull, value); }
        }

        #endregion

        #region Map Contact Module

        public static string MapCachPath
        {
            get { return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringMapCachPath); }
            set { Settings.SetValue(EnumPOCAppSettings.StringMapCachPath, value); }
        }

        #endregion

        #region Profile

        public static int ModuleLastItemType
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntModuleLastItemType); }
            set { Settings.SetValue(EnumPOCAppSettings.IntModuleLastItemType, value); }
        }

        #endregion

        public static string KarsunLastAttachePath
        {
            get
            {
                return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringKarsunLastAttachePath);
            }
            set { Settings.SetValue(EnumPOCAppSettings.StringKarsunLastAttachePath, value); }
        }

        #region IApplicationSettings

        private static IApplicationSettings _settings;

        private static IApplicationSettings Settings
        {
            get { return _settings ?? (_settings = ServiceLocator.Current.GetInstance<IApplicationSettings>()); }
        }

        #endregion

        #region Network

        public static string ServerName
        {
            get { return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringServerName); }
            set { Settings.SetValue(EnumPOCAppSettings.StringServerName, value); }
        }

        public static string ServerPort
        {
            get { return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringServerPort); }
            set { Settings.SetValue(EnumPOCAppSettings.StringServerPort, value); }
        }

        public static bool UseMSSQLServer2
        {
            get { return Settings.GetValue<bool, EnumPOCAppSettings>(EnumPOCAppSettings.BoolUseMSSQLServer2); }
            set { Settings.SetValue(EnumPOCAppSettings.BoolUseMSSQLServer2, value); }
        }

        #endregion

        #region Application UI

        public static int LanguageLCID
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntApplicationLCID); }
            set { Settings.SetValue(EnumPOCAppSettings.IntApplicationLCID, value); }
        }

        public static string ApplicationDirection
        {
            get { return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringApplicationDirection); }
            set { Settings.SetValue(EnumPOCAppSettings.StringApplicationDirection, value); }
        }

        public static string ApplicationFontFamily
        {
            get
            {
                return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringApplicationFontFamily);
            }
            set { Settings.SetValue(EnumPOCAppSettings.StringApplicationFontFamily, value); }
        }

        public static int ApplicationFontSize
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntApplicationFontSize); }
            set { Settings.SetValue(EnumPOCAppSettings.IntApplicationFontSize, value); }
        }

        #endregion

        #region Keyboard Layout

        public static string ApplicationKBLayoutDefault
        {
            get { return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringDefLangLayoutName); }
            set { Settings.SetValue(EnumPOCAppSettings.StringDefLangLayoutName, value); }
        }

        public static string ApplicationKBLayoutRTL
        {
            get { return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringRTLLangLayoutName); }
            set { Settings.SetValue(EnumPOCAppSettings.StringRTLLangLayoutName, value); }
        }

        public static string ApplicationKBLayoutLTR
        {
            get { return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringLTRLangLayoutName); }
            set { Settings.SetValue(EnumPOCAppSettings.StringLTRLangLayoutName, value); }
        }

        #endregion

        #region DateTime and Calendar

        public static int ApplicationCalendar
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntApplicationCalendar); }
            set { Settings.SetValue(EnumPOCAppSettings.IntApplicationCalendar, value); }
        }

        public static string ApplicationDateFormat
        {
            get { return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringDateTimeFormat); }
            set { Settings.SetValue(EnumPOCAppSettings.StringDateTimeFormat, value); }
        }

        public static int HijriCalendarOffset
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntHijriCalendarOffset); }
            set { Settings.SetValue(EnumPOCAppSettings.IntHijriCalendarOffset, value); }
        }

        #endregion

        #region Dock

        public static bool DockIsFixed
        {
            get { return Settings.GetValue<bool, EnumPOCAppSettings>(EnumPOCAppSettings.BoolDockIsFixed); }
            set { Settings.SetValue(EnumPOCAppSettings.BoolDockIsFixed, value); }
        }

        public static int DockWidth
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntDockWidth); }
            set { Settings.SetValue(EnumPOCAppSettings.IntDockWidth, value); }
        }

        #endregion

        #region Membership

        public static string UserName
        {
            get { return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringUserName); }
            set { Settings.SetValue(EnumPOCAppSettings.StringUserName, value); }
        }

        public static string Password
        {
            get
            {
                return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringPassword, false, true);
            }
            set { Settings.SetValue(EnumPOCAppSettings.StringPassword, value, true); }
        }

        public static bool SaveUserName
        {
            get { return Settings.GetValue<bool, EnumPOCAppSettings>(EnumPOCAppSettings.BoolSaveUserName); }
            set { Settings.SetValue(EnumPOCAppSettings.BoolSaveUserName, value); }
        }

        public static bool SavePassword
        {
            get { return Settings.GetValue<bool, EnumPOCAppSettings>(EnumPOCAppSettings.BoolSavePassword); }
            set { Settings.SetValue(EnumPOCAppSettings.BoolSavePassword, value); }
        }

        public static bool AutoLogin
        {
            get { return Settings.GetValue<bool, EnumPOCAppSettings>(EnumPOCAppSettings.BoolAutoLogin); }
            set { Settings.SetValue(EnumPOCAppSettings.BoolAutoLogin, value); }
        }

        #endregion

        #region PrayTime

        public static string PrayTimeTimeZoneID
        {
            get { return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringPrayTimeTimeZoneID); }
            set { Settings.SetValue(EnumPOCAppSettings.StringPrayTimeTimeZoneID, value); }
        }

        public static int PrayTimeLatitude
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntPrayTimeLatitude); }
            set { Settings.SetValue(EnumPOCAppSettings.IntPrayTimeLatitude, value); }
        }

        public static int PrayTimeLongitude
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntPrayTimeLongitude); }
            set { Settings.SetValue(EnumPOCAppSettings.IntPrayTimeLongitude, value); }
        }

        public static int PrayTimeCalculationMethod
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntPrayTimeCalculationMethod); }
            set { Settings.SetValue(EnumPOCAppSettings.IntPrayTimeCalculationMethod, value); }
        }

        public static int PrayTimeJuristicMethod
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntPrayTimeJuristicMethod); }
            set { Settings.SetValue(EnumPOCAppSettings.IntPrayTimeJuristicMethod, value); }
        }

        public static int PrayTimeHigherLatitude
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntPrayTimeHigherLatitude); }
            set { Settings.SetValue(EnumPOCAppSettings.IntPrayTimeHigherLatitude, value); }
        }

        #endregion

        #region Contact Module

        public static string ContactDateFormat
        {
            get { return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringContactDateFormat); }
            set { Settings.SetValue(EnumPOCAppSettings.StringContactDateFormat, value); }
        }

        public static int ModuleContentWidth
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntModuleContentWidth); }
            set { Settings.SetValue(EnumPOCAppSettings.IntModuleContentWidth, value); }
        }

        public static string ContactColumnWidth
        {
            get { return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringContactColumnWidth); }
            set { Settings.SetValue(EnumPOCAppSettings.StringContactColumnWidth, value); }
        }

        #endregion

        #region Call Log

        public static string CallDateFormat
        {
            get { return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringCallDateFormat); }
            set { Settings.SetValue(EnumPOCAppSettings.StringCallDateFormat, value); }
        }

        public static EnumCallTeleCodeDisplayMode CallTeleCodeDisplayMode
        {
            get
            {
                return
                    Settings.GetValue<EnumCallTeleCodeDisplayMode, EnumPOSAppSettings>(
                        EnumPOSAppSettings.EnumCallTeleCodeDisplayMode);
            }
            set { Settings.SetValue(EnumPOSAppSettings.EnumCallTeleCodeDisplayMode, value); }
        }

        public static EnumCallLineDisplayMode CallLineDisplayMode
        {
            get
            {
                return
                    Settings.GetValue<EnumCallLineDisplayMode, EnumPOSAppSettings>(
                        EnumPOSAppSettings.EnumCallLineDisplayMode);
            }
            set { Settings.SetValue(EnumPOSAppSettings.EnumCallLineDisplayMode, value); }
        }

        public static EnumCallLineDisplayMode CallExtDisplayMode
        {
            get
            {
                return
                    Settings.GetValue<EnumCallLineDisplayMode, EnumPOSAppSettings>(
                        EnumPOSAppSettings.EnumCallExtDisplayMode);
            }
            set { Settings.SetValue(EnumPOSAppSettings.EnumCallExtDisplayMode, value); }
        }

        public static bool CallIsAutoRefresh
        {
            get { return Settings.GetValue<bool, EnumPOCAppSettings>(EnumPOCAppSettings.BoolCallIsAutoRefresh); }
            set { Settings.SetValue(EnumPOCAppSettings.BoolCallIsAutoRefresh, value); }
        }

        public static int CallAutoRefreshIndex
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntCallAutoRefreshIndex); }
            set { Settings.SetValue(EnumPOCAppSettings.IntCallAutoRefreshIndex, value); }
        }

        #endregion

        #region Email

        public static int EmailSpliterWidth
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntEmailSpliterWidth); }
            set { Settings.SetValue(EnumPOCAppSettings.IntEmailSpliterWidth, value); }
        }

        public static int EmailSpliterBodyWidth
        {
            get { return Settings.GetValue<int, EnumPOCAppSettings>(EnumPOCAppSettings.IntEmailSpliterBodyWidth); }
            set { Settings.SetValue(EnumPOCAppSettings.IntEmailSpliterBodyWidth, value); }
        }

        public static bool EmailNavIsExpanded
        {
            get { return Settings.GetValue<bool, EnumPOCAppSettings>(EnumPOCAppSettings.BoolEmailNavIsExpanded); }
            set { Settings.SetValue(EnumPOCAppSettings.BoolEmailNavIsExpanded, value); }
        }

        public static string EmailLastAttachePath
        {
            get { return Settings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringEmailLastAttachePath); }
            set { Settings.SetValue(EnumPOCAppSettings.StringEmailLastAttachePath, value); }
        }

        #endregion
    }
}
