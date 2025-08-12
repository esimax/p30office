using System;
using System.Runtime.Serialization;
using POL.Lib.Interfaces.Info;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public class ServerToClientInformation
    {
        #region CTOR

        public ServerToClientInformation()
        {
            Device = EnumDeviceUsed.None;
            ServerDate = DateTime.Now;
        }

        #endregion

        [DataMember]
        public DateTime ServerDate { get; set; }

        [DataMember]
        public int ServerCount { get; set; }

        #region P30Office

        [DataMember]
        public string ServerVersion { get; set; }

        #endregion

        #region OS Information

        [DataMember]
        public InfoOS OSInformation { get; set; }

        #endregion

        #region Processor Information

        [DataMember]
        public InfoCPU CPUInformation { get; set; }

        #endregion

        #region LogicalDisk Information

        [DataMember]
        public InfoLogicalDisk[] LogicalDiskInformations { get; set; }

        #endregion

        #region RAM Information

        [DataMember]
        public ulong TotalRam { get; set; }

        #endregion

        #region System Information

        [DataMember]
        public InfoSystem SystemInformation { get; set; }

        #endregion

        #region Network Information

        [DataMember]
        public InfoNetwork[] NetworkInformations { get; set; }

        #endregion

        [DataMember]
        public bool SyncEnable { get; set; }

        [DataMember]
        public int SyncCode { get; set; }


        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Company { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string IntCode { get; set; }

        [DataMember]
        public string EcoCode { get; set; }

        [DataMember]
        public string SarbargPath { get; set; }


        [DataMember]
        public bool IsTamas { get; set; }

        [DataMember]
        public string SmsMultiSettingsJsonString { get; set; }

        #region Database Info

        [DataMember]
        public EnumDatabaseProvider DatabaseProvider { get; set; }

        #region MSSQL

        [DataMember]
        public string MSSQLServer { get; set; }

        [DataMember]
        public string MSSQLServer2 { get; set; }

        [DataMember]
        public string MSSQLDatabase { get; set; }

        [DataMember]
        public bool MSSQLWindowsAuthorization { get; set; }

        [DataMember]
        public string MSSQLUserName { get; set; }

        [DataMember]
        public string MSSQLPassword { get; set; }

        #endregion

        #region MSSQLCE

        [DataMember]
        public string MSSQLCEFileName { get; set; }

        #endregion

        #endregion

        #region Telephony

        [DataMember]
        public Guid CurrentCityGuid { get; set; }

        [DataMember]
        public string MobileStartingCode { get; set; }

        [DataMember]
        public int MobileLength { get; set; }

        [DataMember]
        public string MobileDefaultTitle { get; set; }


        [DataMember]
        public string PhoneNumberDefaultTitle { get; set; }

        [DataMember]
        public string LineNames { get; set; }

        [DataMember]
        public string ExtNames { get; set; }

        #endregion

        #region Device Info

        [DataMember]
        public EnumDeviceUsed Device { get; set; }

        [DataMember]
        public bool DeviceHasRecord { get; set; }

        [DataMember]
        public string DeviceRecordPath { get; set; }

        [DataMember]
        public bool DeviceHasVoiceMessage { get; set; }

        [DataMember]
        public string DeviceExtraInformation { get; set; }

        #endregion

        #region Monitoring Status

        [DataMember]
        public EnumStatusTelecommunication StatusTelecommunication { get; set; }

        [DataMember]
        public EnumStatusDeviceALM StatusDeviceALM { get; set; }

        [DataMember]
        public EnumStatusDevicePana StatusDevicePana { get; set; }

        [DataMember]
        [Obsolete]
        public EnumStatusDeviceVirtual StatusDeviceVirtual { get; set; }

        [DataMember]
        public EnumStatusDeviceTelsa StatusDeviceTelsa { get; set; }

        [DataMember]
        public EnumStatusMembership StatusMembership { get; set; }

        [DataMember]
        public EnumStatusPhoneMonitoring StatusPhoneMonitoring { get; set; }

        #endregion

        #region Contact Custom Columns

        #region Col0

        [DataMember]
        public bool ContactCustColEnable0 { get; set; }

        [DataMember]
        public string ContactCustColTitle0 { get; set; }

        [DataMember]
        public string ContactCustColOid0 { get; set; }

        #endregion

        #region Col1

        [DataMember]
        public bool ContactCustColEnable1 { get; set; }

        [DataMember]
        public string ContactCustColTitle1 { get; set; }

        [DataMember]
        public string ContactCustColOid1 { get; set; }

        #endregion

        #region Col2

        [DataMember]
        public bool ContactCustColEnable2 { get; set; }

        [DataMember]
        public string ContactCustColTitle2 { get; set; }

        [DataMember]
        public string ContactCustColOid2 { get; set; }

        #endregion

        #region Col3

        [DataMember]
        public bool ContactCustColEnable3 { get; set; }

        [DataMember]
        public string ContactCustColTitle3 { get; set; }

        [DataMember]
        public string ContactCustColOid3 { get; set; }

        #endregion

        #region Col4

        [DataMember]
        public bool ContactCustColEnable4 { get; set; }

        [DataMember]
        public string ContactCustColTitle4 { get; set; }

        [DataMember]
        public string ContactCustColOid4 { get; set; }

        #endregion

        #region Col5

        [DataMember]
        public bool ContactCustColEnable5 { get; set; }

        [DataMember]
        public string ContactCustColTitle5 { get; set; }

        [DataMember]
        public string ContactCustColOid5 { get; set; }

        #endregion

        #region Col6

        [DataMember]
        public bool ContactCustColEnable6 { get; set; }

        [DataMember]
        public string ContactCustColTitle6 { get; set; }

        [DataMember]
        public string ContactCustColOid6 { get; set; }

        #endregion

        #region Col7

        [DataMember]
        public bool ContactCustColEnable7 { get; set; }

        [DataMember]
        public string ContactCustColTitle7 { get; set; }

        [DataMember]
        public string ContactCustColOid7 { get; set; }

        #endregion

        #region Col8

        [DataMember]
        public bool ContactCustColEnable8 { get; set; }

        [DataMember]
        public string ContactCustColTitle8 { get; set; }

        [DataMember]
        public string ContactCustColOid8 { get; set; }

        #endregion

        #region Col9

        [DataMember]
        public bool ContactCustColEnable9 { get; set; }

        [DataMember]
        public string ContactCustColTitle9 { get; set; }

        [DataMember]
        public string ContactCustColOid9 { get; set; }

        #endregion

        #endregion
    }
}
