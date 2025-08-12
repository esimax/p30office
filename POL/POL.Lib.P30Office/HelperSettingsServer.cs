using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;

namespace POL.Lib.XOffice
{
    public class HelperSettingsServer
    {
        public static string SmsDeviceList
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSmsDeviceList); }
            set { Settings.SetValue(EnumPOSAppSettings.StringSmsDeviceList, value); }
        }

        public static string ResetAdminPass
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringResetAdminPass, false,
                    true);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringResetAdminPass, value, true); }
        }

        #region IApplicationSettings

        public static IApplicationSettings _settings;

        public static IApplicationSettings Settings
        {
            get { return _settings ?? (_settings = ServiceLocator.Current.GetInstance<IApplicationSettings>()); }
        }
        #endregion

        #region Service

        public static string POSVersion
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringPOSVersion); }
            set { Settings.SetValue(EnumPOSAppSettings.StringPOSVersion, value); }
        }

        public static int POSDelayIndex
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntPOSDelayIndex); }
            set { Settings.SetValue(EnumPOSAppSettings.IntPOSDelayIndex, value); }
        }

        public static bool POSDelayForced
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolPOSDelayForced); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolPOSDelayForced, value); }
        }

        public static int ServiceExecutionCount
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntServiceExecutionCount); }
            set { Settings.SetValue(EnumPOSAppSettings.IntServiceExecutionCount, value); }
        }

        #endregion

        #region Monitoring Status

        public static EnumStatusLicense StatusLicense
        {
            get
            {
                return Settings.GetValue<EnumStatusLicense, EnumPOSAppSettings>(EnumPOSAppSettings.EnumStatusLicense);
            }
            set { Settings.SetValue(EnumPOSAppSettings.EnumStatusLicense, value); }
        }

        public static EnumStatusDatabase StatusDatabase
        {
            get
            {
                return Settings.GetValue<EnumStatusDatabase, EnumPOSAppSettings>(EnumPOSAppSettings.EnumStatusDatabase);
            }
            set { Settings.SetValue(EnumPOSAppSettings.EnumStatusDatabase, value); }
        }

        public static EnumStatusNetwork StatusNetwork
        {
            get
            {
                return Settings.GetValue<EnumStatusNetwork, EnumPOSAppSettings>(EnumPOSAppSettings.EnumStatusNetwork);
            }
            set { Settings.SetValue(EnumPOSAppSettings.EnumStatusNetwork, value); }
        }

        public static EnumStatusTelecommunication StatusTelecommunication
        {
            get
            {
                return
                    Settings.GetValue<EnumStatusTelecommunication, EnumPOSAppSettings>(
                        EnumPOSAppSettings.EnumStatusTelecommunication);
            }
            set { Settings.SetValue(EnumPOSAppSettings.EnumStatusTelecommunication, value); }
        }

        public static EnumStatusDeviceALM StatusDeviceALM
        {
            get
            {
                return Settings.GetValue<EnumStatusDeviceALM, EnumPOSAppSettings>(EnumPOSAppSettings.EnumStatusDeviceALM);
            }
            set { Settings.SetValue(EnumPOSAppSettings.EnumStatusDeviceALM, value); }
        }

        public static EnumStatusDeviceTelsa StatusDeviceTelsa
        {
            get
            {
                return
                    Settings.GetValue<EnumStatusDeviceTelsa, EnumPOSAppSettings>(
                        EnumPOSAppSettings.EnumStatusDeviceTelsa);
            }
            set { Settings.SetValue(EnumPOSAppSettings.EnumStatusDeviceTelsa, value); }
        }

        public static EnumStatusDevicePana StatusDevicePana
        {
            get
            {
                return
                    Settings.GetValue<EnumStatusDevicePana, EnumPOSAppSettings>(EnumPOSAppSettings.EnumStatusDevicePana);
            }
            set { Settings.SetValue(EnumPOSAppSettings.EnumStatusDevicePana, value); }
        }

        public static EnumStatusPhoneMonitoring StatusPhoneMonitoring
        {
            get
            {
                return
                    Settings.GetValue<EnumStatusPhoneMonitoring, EnumPOSAppSettings>(
                        EnumPOSAppSettings.EnumStatusPhoneMonitoring);
            }
            set { Settings.SetValue(EnumPOSAppSettings.EnumStatusPhoneMonitoring, value); }
        }

        public static EnumStatusMembership StatusMembership
        {
            get
            {
                return
                    Settings.GetValue<EnumStatusMembership, EnumPOSAppSettings>(EnumPOSAppSettings.EnumStatusMembership);
            }
            set { Settings.SetValue(EnumPOSAppSettings.EnumStatusMembership, value); }
        }

        public static EnumStatusMessaging StatusMessaging
        {
            get
            {
                return Settings.GetValue<EnumStatusMessaging, EnumPOSAppSettings>(EnumPOSAppSettings.EnumStatusMessaging);
            }
            set { Settings.SetValue(EnumPOSAppSettings.EnumStatusMessaging, value); }
        }

        public static EnumStatusEmail StatusEmail
        {
            get { return Settings.GetValue<EnumStatusEmail, EnumPOSAppSettings>(EnumPOSAppSettings.EnumStatusEmail); }
            set { Settings.SetValue(EnumPOSAppSettings.EnumStatusEmail, value); }
        }

        public static EnumStatusSMS StatusSMS
        {
            get { return Settings.GetValue<EnumStatusSMS, EnumPOSAppSettings>(EnumPOSAppSettings.EnumStatusSMS); }
            set { Settings.SetValue(EnumPOSAppSettings.EnumStatusSMS, value); }
        }

        #endregion

        #region Database

        public static EnumDatabaseProvider DatabaseProvider
        {
            get
            { return Settings.GetValue<EnumDatabaseProvider, EnumPOSAppSettings>(EnumPOSAppSettings.EnumDatabaseProvider); }
            set { Settings.SetValue(EnumPOSAppSettings.EnumDatabaseProvider, value); }
        }

        public static string MSSQLServer
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringMSSQLServer); }
            set { Settings.SetValue(EnumPOSAppSettings.StringMSSQLServer, value); }
        }

        public static string MSSQLDatabase
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringMSSQLDatabase); }
            set { Settings.SetValue(EnumPOSAppSettings.StringMSSQLDatabase, value); }
        }

        public static bool MSSQLWindowsAuthorization
        {
            get
            {
                return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolMSSQLWindowsAuthorization);
            }
            set { Settings.SetValue(EnumPOSAppSettings.BoolMSSQLWindowsAuthorization, value); }
        }

        public static string MSSQLUserName
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringMSSQLUserName); }
            set { Settings.SetValue(EnumPOSAppSettings.StringMSSQLUserName, value); }
        }

        public static string MSSQLPassword
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringMSSQLPassword, false, true);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringMSSQLPassword, value, true); }
        }

        public static string MSSQLServer2
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringMSSQLServer2); }
            set { Settings.SetValue(EnumPOSAppSettings.StringMSSQLServer2, value); }
        }

        public static bool MSSQLOptimizeForce
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolMSSQLOptimizeForce); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolMSSQLOptimizeForce, value); }
        }

        public static bool MSSQLOptimizeActive
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolMSSQLOptimizeActive); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolMSSQLOptimizeActive, value); }
        }

        public static bool MSSQLBackupAuto
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolMSSQLBackupAuto); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolMSSQLBackupAuto, value); }
        }

        public static int MSSQLBackupTime
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntMSSQLBackupTime); }
            set { Settings.SetValue(EnumPOSAppSettings.IntMSSQLBackupTime, value); }
        }

        public static bool MSSQLBackupWeekly
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolMSSQLBackupWeekly); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolMSSQLBackupWeekly, value); }
        }

        public static int MSSQLBackupDayOfWeek
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntMSSQLBackupDayOfWeek); }
            set { Settings.SetValue(EnumPOSAppSettings.IntMSSQLBackupDayOfWeek, value); }
        }

        public static bool MSSQLAutoReindex
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolMSSQLAutoReindex); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolMSSQLAutoReindex, value); }
        }

        public static int MSSQLOptimizeIntervalMinutes
        {
            get
            {
                return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntMSSQLOptimizeIntervalMinutes);
            }
            set { Settings.SetValue(EnumPOSAppSettings.IntMSSQLOptimizeIntervalMinutes, value); }
        }


        public static string AccessDatabasePath
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringAccessDatabasePath); }
            set { Settings.SetValue(EnumPOSAppSettings.StringAccessDatabasePath, value); }
        }

        public static string XMLDatabasePath
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringXMLDatabasePath); }
            set { Settings.SetValue(EnumPOSAppSettings.StringXMLDatabasePath, value); }
        }

        public static string MSSQLCEPath
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringMSSQLCEPath); }
            set { Settings.SetValue(EnumPOSAppSettings.StringMSSQLCEPath, value); }
        }

        public static bool CallAutoSync
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolCallAutoSync); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolCallAutoSync, value); }
        }

        public static bool CallAutoCalcStat
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolCallAutoCalcStat); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolCallAutoCalcStat, value); }
        }

        public static long CallAutoCalcStatLastDate
        {
            get
            {
                return Settings.GetValue<long, EnumPOSAppSettings>(EnumPOSAppSettings.Int64CallAutoCalcStatLastDate);
            }
            set { Settings.SetValue(EnumPOSAppSettings.Int64CallAutoCalcStatLastDate, value); }
        }

        #endregion

        #region License

        public static string LicenseSerial
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringLicenseSerial); }
            set { Settings.SetValue(EnumPOSAppSettings.StringLicenseSerial, value); }
        }

        public static string LicenseOwner
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringLicenseOwner); }
            set { Settings.SetValue(EnumPOSAppSettings.StringLicenseOwner, value); }
        }

        public static string LicenseCompany
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringLicenseCompany); }
            set { Settings.SetValue(EnumPOSAppSettings.StringLicenseCompany, value); }
        }

        public static string LicenseMobile
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringLicenseMobile); }
            set { Settings.SetValue(EnumPOSAppSettings.StringLicenseMobile, value); }
        }

        public static string LicensePhone
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringLicensePhone); }
            set { Settings.SetValue(EnumPOSAppSettings.StringLicensePhone, value); }
        }

        public static string LicenseEmail
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringLicenseEmail); }
            set { Settings.SetValue(EnumPOSAppSettings.StringLicenseEmail, value); }
        }

        public static string LicenseAddress
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringLicenseAddress); }
            set { Settings.SetValue(EnumPOSAppSettings.StringLicenseAddress, value); }
        }

        public static string LicenseType
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringLicenseType); }
            set { Settings.SetValue(EnumPOSAppSettings.StringLicenseType, value); }
        }

        public static string LicenseAgentCode
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringLicenseAgentCode); }
            set { Settings.SetValue(EnumPOSAppSettings.StringLicenseAgentCode, value); }
        }

        public static string LicenseDate
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringLicenseDate); }
            set { Settings.SetValue(EnumPOSAppSettings.StringLicenseDate, value); }
        }

        public static string LicenseVersion
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringLicenseVersion); }
            set { Settings.SetValue(EnumPOSAppSettings.StringLicenseVersion, value); }
        }

        public static string infoName
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringinfoName); }
            set { Settings.SetValue(EnumPOSAppSettings.StringinfoName, value); }
        }

        public static string infoCompany
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringinfoCompany); }
            set { Settings.SetValue(EnumPOSAppSettings.StringinfoCompany, value); }
        }

        public static string infoEmail
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringinfoEmail); }
            set { Settings.SetValue(EnumPOSAppSettings.StringinfoEmail, value); }
        }

        public static string infoPhone
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringinfoPhone); }
            set { Settings.SetValue(EnumPOSAppSettings.StringinfoPhone, value); }
        }

        public static string infoMobile
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringinfoMobile); }
            set { Settings.SetValue(EnumPOSAppSettings.StringinfoMobile, value); }
        }

        public static string infoAddress
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringinfoAddress); }
            set { Settings.SetValue(EnumPOSAppSettings.StringinfoAddress, value); }
        }

        #endregion

        #region Network

        public static string ServerName
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringServerName); }
            set { Settings.SetValue(EnumPOSAppSettings.StringServerName, value); }
        }

        public static string ServerPort
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringServerPort); }
            set { Settings.SetValue(EnumPOSAppSettings.StringServerPort, value); }
        }

        public static bool SyncEnable
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSyncEnable); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSyncEnable, value); }
        }

        public static string SyncServerUrl
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSyncServerUrl); }
            set { Settings.SetValue(EnumPOSAppSettings.StringSyncServerUrl, value); }
        }

        public static int SyncDurationIndex
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntSyncDurationIndex); }
            set { Settings.SetValue(EnumPOSAppSettings.IntSyncDurationIndex, value); }
        }

        public static int SyncCode
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntSyncCode); }
            set { Settings.SetValue(EnumPOSAppSettings.IntSyncCode, value); }
        }


        public static string FirstName
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringFirstName); }
            set { Settings.SetValue(EnumPOSAppSettings.StringFirstName, value); }
        }

        public static string LastName
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringLastName); }
            set { Settings.SetValue(EnumPOSAppSettings.StringLastName, value); }
        }

        public static string Company
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCompany); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCompany, value); }
        }

        public static string PhoneNumber
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringPhoneNumber); }
            set { Settings.SetValue(EnumPOSAppSettings.StringPhoneNumber, value); }
        }

        public static string Address
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringAddress); }
            set { Settings.SetValue(EnumPOSAppSettings.StringAddress, value); }
        }

        public static string IntCode
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringIntCode); }
            set { Settings.SetValue(EnumPOSAppSettings.StringIntCode, value); }
        }

        public static string EcoCode
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringEcoCode); }
            set { Settings.SetValue(EnumPOSAppSettings.StringEcoCode, value); }
        }

        public static string SarbargPath
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSarbargPath); }
            set { Settings.SetValue(EnumPOSAppSettings.StringSarbargPath, value); }
        }

        #endregion

        #region Telecommunication

        public static string CurrentCityOid
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCurrentCityOid); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCurrentCityOid, value); }
        }

        public static string MobileStartingCode
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringMobileStartingCode); }
            set { Settings.SetValue(EnumPOSAppSettings.StringMobileStartingCode, value); }
        }

        public static int MobileLength
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntMobileLength); }
            set { Settings.SetValue(EnumPOSAppSettings.IntMobileLength, value); }
        }

        public static string MobileDefaultTitle
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringMobileDefaultTitle); }
            set { Settings.SetValue(EnumPOSAppSettings.StringMobileDefaultTitle, value); }
        }

        public static string PhoneNumberDefaultTitle
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringPhoneNumberDefaultTitle);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringPhoneNumberDefaultTitle, value); }
        }

        public static string LineNames
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringLineNames); }
            set { Settings.SetValue(EnumPOSAppSettings.StringLineNames, value); }
        }

        #endregion

        #region Device ALM

        public static bool DeviceALMEnable
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDeviceALMEnable); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDeviceALMEnable, value); }
        }

        public static bool DeviceALMLog
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDeviceALMLog); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDeviceALMLog, value); }
        }

        public static bool DeviceAlmFullLog
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDeviceAlmFullLog); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDeviceAlmFullLog, value); }
        }

        public static bool DeviceALMActive1
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDeviceALMActive1); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDeviceALMActive1, value); }
        }

        public static string DeviceALMPort1
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDeviceALMPort1); }
            set { Settings.SetValue(EnumPOSAppSettings.StringDeviceALMPort1, value); }
        }

        public static int DeviceALMLineCount1
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntDeviceALMLineCount1); }
            set { Settings.SetValue(EnumPOSAppSettings.IntDeviceALMLineCount1, value); }
        }

        public static bool DeviceALMActive2
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDeviceALMActive2); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDeviceALMActive2, value); }
        }

        public static string DeviceALMPort2
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDeviceALMPort2); }
            set { Settings.SetValue(EnumPOSAppSettings.StringDeviceALMPort2, value); }
        }

        public static int DeviceALMLineCount2
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntDeviceALMLineCount2); }
            set { Settings.SetValue(EnumPOSAppSettings.IntDeviceALMLineCount2, value); }
        }

        public static bool DeviceALMActive3
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDeviceALMActive3); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDeviceALMActive3, value); }
        }

        public static string DeviceALMPort3
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDeviceALMPort3); }
            set { Settings.SetValue(EnumPOSAppSettings.StringDeviceALMPort3, value); }
        }

        public static int DeviceALMLineCount3
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntDeviceALMLineCount3); }
            set { Settings.SetValue(EnumPOSAppSettings.IntDeviceALMLineCount3, value); }
        }

        public static bool DeviceALMActive4
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDeviceALMActive4); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDeviceALMActive4, value); }
        }

        public static string DeviceALMPort4
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDeviceALMPort4); }
            set { Settings.SetValue(EnumPOSAppSettings.StringDeviceALMPort4, value); }
        }

        public static int DeviceALMLineCount4
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntDeviceALMLineCount4); }
            set { Settings.SetValue(EnumPOSAppSettings.IntDeviceALMLineCount4, value); }
        }

        public static bool DeviceALMBlockCallOut
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDeviceALMBlockCallOut); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDeviceALMBlockCallOut, value); }
        }

        public static string DeviceALMBlockCallOutData
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDeviceALMBlockCallOutData);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringDeviceALMBlockCallOutData, value); }
        }

        public static bool DeviceALMBlockCallIn
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDeviceALMBlockCallIn); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDeviceALMBlockCallIn, value); }
        }

        public static string DeviceALMBlockCallInData
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDeviceALMBlockCallInData);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringDeviceALMBlockCallInData, value); }
        }

        public static string DeviceALMRecordPath
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDeviceALMRecordPath); }
            set { Settings.SetValue(EnumPOSAppSettings.StringDeviceALMRecordPath, value); }
        }

        #endregion

        #region Device Panasonic

        public static bool DevicePanaEnable
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDevicePanaEnable); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDevicePanaEnable, value); }
        }

        public static bool DevicePanaLog
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDevicePanaLog); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDevicePanaLog, value); }
        }

        public static bool DevicePanaFullLog
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDevicePanaFullLog); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDevicePanaFullLog, value); }
        }

        public static string DevicePanaPort1
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDevicePanaPort1); }
            set { Settings.SetValue(EnumPOSAppSettings.StringDevicePanaPort1, value); }
        }

        public static int DevicePanaType1
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntDevicePanaType1); }
            set { Settings.SetValue(EnumPOSAppSettings.IntDevicePanaType1, value); }
        }

        public static int DevicePanaBaud1
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntDevicePanaBaud1); }
            set { Settings.SetValue(EnumPOSAppSettings.IntDevicePanaBaud1, value); }
        }

        public static int DevicePanaDateFormat1
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntDevicePanaDateFormat1); }
            set { Settings.SetValue(EnumPOSAppSettings.IntDevicePanaDateFormat1, value); }
        }

        public static string DevicePanaMapping1
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDevicePanaMapping1); }
            set { Settings.SetValue(EnumPOSAppSettings.StringDevicePanaMapping1, value); }
        }

        public static string DevicePanaInternal1
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDevicePanaInternal1); }
            set { Settings.SetValue(EnumPOSAppSettings.StringDevicePanaInternal1, value); }
        }

        public static bool DevicePanaIpexEnable
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDevicePanaIpexEnable); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDevicePanaIpexEnable, value); }
        }

        public static string DevicePanaIpexAddress
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDevicePanaIpexAddress);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringDevicePanaIpexAddress, value); }
        }

        public static int DevicePanaIpexPort
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntDevicePanaIpexPort); }
            set { Settings.SetValue(EnumPOSAppSettings.IntDevicePanaIpexPort, value); }
        }

        public static bool DevicePanaVirtualEnable
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDevicePanaVirtualEnable); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDevicePanaVirtualEnable, value); }
        }






        public static string DevicePanaRecordPath
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDevicePanaRecordPath); }
            set { Settings.SetValue(EnumPOSAppSettings.StringDevicePanaRecordPath, value); }
        }

        public static bool DevicePanaMultiExtEnable
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDevicePanaMultiExtEnable); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDevicePanaMultiExtEnable, value); }
        }

        public static string DevicePanaMultiExtGroupCode
        {
            get
            {
                return
                    Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDevicePanaMultiExtGroupCode);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringDevicePanaMultiExtGroupCode, value); }
        }

        public static string DevicePanaMultiExtCodes
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDevicePanaMultiExtCodes);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringDevicePanaMultiExtCodes, value); }
        }

        #endregion

        #region Device Telsa

        public static bool DeviceTelsaEnable
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDeviceTelsaEnable); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDeviceTelsaEnable, value); }
        }

        public static bool DeviceTelsaLog
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDeviceTelsaLog); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDeviceTelsaLog, value); }
        }

        public static string DeviceTelsaServerName
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDeviceTelsaServerName);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringDeviceTelsaServerName, value); }
        }

        public static string DeviceTelsaServerPort
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDeviceTelsaServerPort);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringDeviceTelsaServerPort, value); }
        }

        public static int DeviceTelsaLineCount1
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntDeviceTelsaLineCount1); }
            set { Settings.SetValue(EnumPOSAppSettings.IntDeviceTelsaLineCount1, value); }
        }

        public static int DeviceTelsaLineCount2
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntDeviceTelsaLineCount2); }
            set { Settings.SetValue(EnumPOSAppSettings.IntDeviceTelsaLineCount2, value); }
        }

        public static int DeviceTelsaLineCount3
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntDeviceTelsaLineCount3); }
            set { Settings.SetValue(EnumPOSAppSettings.IntDeviceTelsaLineCount3, value); }
        }

        public static bool DeviceTelsaEnableRecord
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDeviceTelsaEnableRecord); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDeviceTelsaEnableRecord, value); }
        }

        public static int DeviceTelsaPriorityIndex1
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntDeviceTelsaPriorityIndex1); }
            set { Settings.SetValue(EnumPOSAppSettings.IntDeviceTelsaPriorityIndex1, value); }
        }

        public static int DeviceTelsaPriorityIndex2
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntDeviceTelsaPriorityIndex2); }
            set { Settings.SetValue(EnumPOSAppSettings.IntDeviceTelsaPriorityIndex2, value); }
        }

        public static int DeviceTelsaPriorityIndex3
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntDeviceTelsaPriorityIndex3); }
            set { Settings.SetValue(EnumPOSAppSettings.IntDeviceTelsaPriorityIndex3, value); }
        }

        public static bool DeviceTelsaEnableVoiceMessage
        {
            get
            {
                return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDeviceTelsaEnableVoiceMessage);
            }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDeviceTelsaEnableVoiceMessage, value); }
        }

        public static string DeviceTelsaVoiceMessageMapping
        {
            get
            {
                return
                    Settings.GetValue<string, EnumPOSAppSettings>(
                        EnumPOSAppSettings.StringDeviceTelsaVoiceMessageMapping);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringDeviceTelsaVoiceMessageMapping, value); }
        }

        public static string DeviceTelsaRecordPath
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringDeviceTelsaRecordPath);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringDeviceTelsaRecordPath, value); }
        }

        #endregion

        #region Device Virtual

        public static bool DeviceVirtualEnable
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDeviceVirtualEnable); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDeviceVirtualEnable, value); }
        }

        public static bool DeviceVirtualLog
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolDeviceVirtualLog); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolDeviceVirtualLog, value); }
        }

        #endregion

        #region CustomColumn1

        public static bool CustomColumnEnable1
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolCustomColumnEnable1); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolCustomColumnEnable1, value); }
        }

        public static string CustomColumnTitle1
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnTitle1); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnTitle1, value); }
        }

        public static string CustomColumnOid1
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnOid1); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnOid1, value); }
        }

        #endregion

        #region CustomColumn2

        public static bool CustomColumnEnable2
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolCustomColumnEnable2); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolCustomColumnEnable2, value); }
        }

        public static string CustomColumnTitle2
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnTitle2); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnTitle2, value); }
        }

        public static string CustomColumnOid2
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnOid2); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnOid2, value); }
        }

        #endregion

        #region CustomColumn3

        public static bool CustomColumnEnable3
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolCustomColumnEnable3); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolCustomColumnEnable3, value); }
        }

        public static string CustomColumnTitle3
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnTitle3); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnTitle3, value); }
        }

        public static string CustomColumnOid3
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnOid3); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnOid3, value); }
        }

        #endregion

        #region CustomColumn4

        public static bool CustomColumnEnable4
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolCustomColumnEnable4); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolCustomColumnEnable4, value); }
        }

        public static string CustomColumnTitle4
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnTitle4); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnTitle4, value); }
        }

        public static string CustomColumnOid4
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnOid4); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnOid4, value); }
        }

        #endregion

        #region CustomColumn5

        public static bool CustomColumnEnable5
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolCustomColumnEnable5); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolCustomColumnEnable5, value); }
        }

        public static string CustomColumnTitle5
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnTitle5); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnTitle5, value); }
        }

        public static string CustomColumnOid5
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnOid5); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnOid5, value); }
        }

        #endregion

        #region CustomColumn6

        public static bool CustomColumnEnable6
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolCustomColumnEnable6); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolCustomColumnEnable6, value); }
        }

        public static string CustomColumnTitle6
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnTitle6); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnTitle6, value); }
        }

        public static string CustomColumnOid6
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnOid6); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnOid6, value); }
        }

        #endregion

        #region CustomColumn7

        public static bool CustomColumnEnable7
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolCustomColumnEnable7); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolCustomColumnEnable7, value); }
        }

        public static string CustomColumnTitle7
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnTitle7); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnTitle7, value); }
        }

        public static string CustomColumnOid7
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnOid7); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnOid7, value); }
        }

        #endregion

        #region CustomColumn8

        public static bool CustomColumnEnable8
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolCustomColumnEnable8); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolCustomColumnEnable8, value); }
        }

        public static string CustomColumnTitle8
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnTitle8); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnTitle8, value); }
        }

        public static string CustomColumnOid8
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnOid8); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnOid8, value); }
        }

        #endregion

        #region CustomColumn9

        public static bool CustomColumnEnable9
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolCustomColumnEnable9); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolCustomColumnEnable9, value); }
        }

        public static string CustomColumnTitle9
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnTitle9); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnTitle9, value); }
        }

        public static string CustomColumnOid9
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnOid9); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnOid9, value); }
        }

        #endregion

        #region CustomColumn0

        public static bool CustomColumnEnable0
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolCustomColumnEnable0); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolCustomColumnEnable0, value); }
        }

        public static string CustomColumnTitle0
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnTitle0); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnTitle0, value); }
        }

        public static string CustomColumnOid0
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringCustomColumnOid0); }
            set { Settings.SetValue(EnumPOSAppSettings.StringCustomColumnOid0, value); }
        }

        #endregion

        #region Email

        public static bool EmailEnable
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolEmailEnable); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolEmailEnable, value); }
        }

        public static bool EmailLog
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolEmailLog); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolEmailLog, value); }
        }

        public static int EmailUpdateIndex
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntEmailUpdateIndex); }
            set { Settings.SetValue(EnumPOSAppSettings.IntEmailUpdateIndex, value); }
        }

        #endregion

        #region Web

        public static bool WebEnable
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolWebEnable); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolWebEnable, value); }
        }

        public static string WebAddress
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringWebAddress); }
            set { Settings.SetValue(EnumPOSAppSettings.StringWebAddress, value); }
        }

        #endregion

        #region AutoUpdate

        public static bool AutoUpdateEnable
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolAutoUpdateEnable); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolAutoUpdateEnable, value); }
        }

        public static string AutoUpdateLastMessage
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringAutoUpdateLastMessage);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringAutoUpdateLastMessage, value); }
        }

        public static bool AutoUpdateChecking
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolAutoUpdateChecking); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolAutoUpdateChecking, value); }
        }

        public static string AutoUpdateNewVersion
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringAutoUpdateNewVersion); }
            set { Settings.SetValue(EnumPOSAppSettings.StringAutoUpdateNewVersion, value); }
        }

        public static bool AutoUpdateDownloading
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolAutoUpdateDownloading); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolAutoUpdateDownloading, value); }
        }

        public static int AutoUpdatePercent
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntAutoUpdatePercent); }
            set { Settings.SetValue(EnumPOSAppSettings.IntAutoUpdatePercent, value); }
        }

        public static bool AutoUpdateReadyToInstall
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolAutoUpdateReadyToInstall); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolAutoUpdateReadyToInstall, value); }
        }

        public static bool AutoUpdateForced
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolAutoUpdateForced); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolAutoUpdateForced, value); }
        }

        public static bool AutoUpdateUseIEProxy
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolAutoUpdateUseIEProxy); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolAutoUpdateUseIEProxy, value); }
        }

        public static string AutoUpdateFile
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringAutoUpdateFile); }
            set { Settings.SetValue(EnumPOSAppSettings.StringAutoUpdateFile, value); }
        }

        public static string AutoUpdateProxyUsername
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringAutoUpdateProxyUsername);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringAutoUpdateProxyUsername, value); }
        }

        public static string AutoUpdateProxyPassword
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringAutoUpdateProxyPassword,
                    false, true);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringAutoUpdateProxyPassword, value, true); }
        }

        public static string AutoUpdateProxyURL
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringAutoUpdateProxyURL, false,
                    true);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringAutoUpdateProxyURL, value, true); }
        }

        public static string AutoUpdateProxyPort
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringAutoUpdateProxyPort, false,
                    true);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringAutoUpdateProxyPort, value, true); }
        }

        #endregion

        #region SMS

        public static bool SMSEnable
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSMSEnable); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSMSEnable, value); }
        }

        public static bool SMSLog
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSMSLog); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSMSLog, value); }
        }

        public static string SMSNumber
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSMSNumber); }
            set { Settings.SetValue(EnumPOSAppSettings.StringSMSNumber, value); }
        }

        public static string SMSURL
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSMSURL); }
            set { Settings.SetValue(EnumPOSAppSettings.StringSMSURL, value); }
        }

        public static string SMSUserName
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSMSUserName); }
            set { Settings.SetValue(EnumPOSAppSettings.StringSMSUserName, value); }
        }

        public static string SMSPassword
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSMSPassword, false, true);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringSMSPassword, value, true); }
        }

        public static string SMSTestNumber
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSMSTestNumber); }
            set { Settings.SetValue(EnumPOSAppSettings.StringSMSTestNumber, value); }
        }

        public static int SMSScanDelay
        {
            get { return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntSMSScanDelay); }
            set { Settings.SetValue(EnumPOSAppSettings.IntSMSScanDelay, value); }
        }

        public static string SMSRemainCreadit
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSMSRemainCreadit); }
            set { Settings.SetValue(EnumPOSAppSettings.StringSMSRemainCreadit, value); }
        }

        public static string SMSLastMessage
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSMSLastMessage); }
            set { Settings.SetValue(EnumPOSAppSettings.StringSMSLastMessage, value); }
        }

        public static EnumSMSProvider SMSProvider
        {
            get { return Settings.GetValue<EnumSMSProvider, EnumPOSAppSettings>(EnumPOSAppSettings.EnumSMSProvider); }
            set { Settings.SetValue(EnumPOSAppSettings.EnumSMSProvider, value); }
        }

        public static bool SMSDeliveryEnable
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSMSDeliveryEnable); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSMSDeliveryEnable, value); }
        }

        #endregion

        #region SMS

        public static bool SmsFeedbackOnMissedCall
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSmsFeedbackOnMissedCall); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSmsFeedbackOnMissedCall, value); }
        }

        public static string SmsFeedbackOnMissedCallNumber1
        {
            get
            {
                return
                    Settings.GetValue<string, EnumPOSAppSettings>(
                        EnumPOSAppSettings.StringSmsFeedbackOnMissedCallNumber1);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringSmsFeedbackOnMissedCallNumber1, value); }
        }


        public static bool SmsFeedbackOnNotWorkTime
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSmsFeedbackOnNotWorkTime); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSmsFeedbackOnNotWorkTime, value); }
        }

        public static string SmsFeedbackOnNotWorkTimeNumber1
        {
            get
            {
                return
                    Settings.GetValue<string, EnumPOSAppSettings>(
                        EnumPOSAppSettings.StringSmsFeedbackOnNotWorkTimeNumber1);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringSmsFeedbackOnNotWorkTimeNumber1, value); }
        }


        public static bool SmsCallInAnswared
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSmsCallInAnswared); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSmsCallInAnswared, value); }
        }

        public static string SmsCallInAnswaredText
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSmsCallInAnswaredText);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringSmsCallInAnswaredText, value); }
        }

        public static bool SmsCallInAnswaredAllways
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSmsCallInAnswaredAllways); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSmsCallInAnswaredAllways, value); }
        }

        public static bool SmsCallInAnswaredPerday
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSmsCallInAnswaredPerday); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSmsCallInAnswaredPerday, value); }
        }

        public static int SmsCallInAnswaredPerdayCount
        {
            get
            {
                return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntSmsCallInAnswaredPerdayCount);
            }
            set { Settings.SetValue(EnumPOSAppSettings.IntSmsCallInAnswaredPerdayCount, value); }
        }

        public static bool SmsCallInNotAnswared
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSmsCallInNotAnswared); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSmsCallInNotAnswared, value); }
        }

        public static string SmsCallInNotAnswaredText
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSmsCallInNotAnswaredText);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringSmsCallInNotAnswaredText, value); }
        }

        public static bool SmsCallInNotAnswaredAllways
        {
            get
            {
                return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSmsCallInNotAnswaredAllways);
            }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSmsCallInNotAnswaredAllways, value); }
        }

        public static bool SmsCallInNotAnswaredPerday
        {
            get
            {
                return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSmsCallInNotAnswaredPerday);
            }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSmsCallInNotAnswaredPerday, value); }
        }

        public static int SmsCallInNotAnswaredPerdayCount
        {
            get
            {
                return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntSmsCallInNotAnswaredPerdayCount);
            }
            set { Settings.SetValue(EnumPOSAppSettings.IntSmsCallInNotAnswaredPerdayCount, value); }
        }

        public static bool SmsCallInNotWork
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSmsCallInNotWork); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSmsCallInNotWork, value); }
        }

        public static bool SmsCallInNotWorkMorePriority
        {
            get
            {
                return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSmsCallInNotWorkMorePriority);
            }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSmsCallInNotWorkMorePriority, value); }
        }

        public static string SmsCallInNotWorkText
        {
            get { return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSmsCallInNotWorkText); }
            set { Settings.SetValue(EnumPOSAppSettings.StringSmsCallInNotWorkText, value); }
        }

        public static bool SmsCallInNotWorkAllways
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSmsCallInNotWorkAllways); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSmsCallInNotWorkAllways, value); }
        }

        public static bool SmsCallInNotWorkPerday
        {
            get { return Settings.GetValue<bool, EnumPOSAppSettings>(EnumPOSAppSettings.BoolSmsCallInNotWorkPerday); }
            set { Settings.SetValue(EnumPOSAppSettings.BoolSmsCallInNotWorkPerday, value); }
        }

        public static int SmsCallInNotWorkPerdayCount
        {
            get
            {
                return Settings.GetValue<int, EnumPOSAppSettings>(EnumPOSAppSettings.IntSmsCallInNotWorkPerdayCount);
            }
            set { Settings.SetValue(EnumPOSAppSettings.IntSmsCallInNotWorkPerdayCount, value); }
        }

        public static string SmsCallInNotWork1Start
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSmsCallInNotWork1Start);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringSmsCallInNotWork1Start, value); }
        }

        public static string SmsCallInNotWork1Stop
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSmsCallInNotWork1Stop);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringSmsCallInNotWork1Stop, value); }
        }

        public static string SmsCallInNotWork2Start
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSmsCallInNotWork2Start);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringSmsCallInNotWork2Start, value); }
        }

        public static string SmsCallInNotWork2Stop
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSmsCallInNotWork2Stop);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringSmsCallInNotWork2Stop, value); }
        }

        public static string SmsCallInNotWork3Start
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSmsCallInNotWork3Start);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringSmsCallInNotWork3Start, value); }
        }

        public static string SmsCallInNotWork3Stop
        {
            get
            {
                return Settings.GetValue<string, EnumPOSAppSettings>(EnumPOSAppSettings.StringSmsCallInNotWork3Stop);
            }
            set { Settings.SetValue(EnumPOSAppSettings.StringSmsCallInNotWork3Stop, value); }
        }

        #endregion



    }
}
