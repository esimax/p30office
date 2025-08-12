namespace POL.Lib.XOffice
{
    public enum EnumPOSAppSettings
    {
        StringPOSVersion,
        IntPOSDelayIndex,
        BoolPOSDelayForced,
        IntServiceExecutionCount,


        EnumStatusDatabase,
        EnumDatabaseProvider,
        BoolMSSQLWindowsAuthorization,
        StringMSSQLServer,
        StringMSSQLPassword,
        StringMSSQLUserName,
        StringMSSQLDatabase,
        StringAccessDatabasePath,
        StringMSSQLCEPath,
        StringXMLDatabasePath,
        BoolMSSQLOptimizeForce,
        BoolMSSQLOptimizeActive,
        BoolMSSQLBackupAuto,
        IntMSSQLBackupTime,
        BoolMSSQLBackupWeekly,
        IntMSSQLBackupDayOfWeek,
        BoolMSSQLAutoReindex,
        IntMSSQLOptimizeIntervalMinutes,
        BoolCallAutoSync,
        BoolCallAutoCalcStat,
        Int64CallAutoCalcStatLastDate,
        StringMSSQLServer2,

        EnumStatusLicense,
        StringLicenseSerial,
        StringLicenseOwner,
        StringLicenseCompany,
        StringLicenseMobile,
        StringLicensePhone,
        StringLicenseEmail,
        StringLicenseAddress,
        StringLicenseType,
        StringLicenseAgentCode,
        StringLicenseDate,
        StringLicenseVersion,


        EnumStatusNetwork,
        StringServerName,
        StringServerPort,


        EnumStatusTelecommunication,
        StringCurrentCountryOid,
        StringCurrentCityOid,
        StringMobileStartingCode,
        IntMobileLength,
        StringMobileDefaultTitle,
        StringPhoneNumberDefaultTitle,
        StringLineNames,

        #region Device Alm

        EnumStatusDeviceALM,
        BoolDeviceALMEnable,
        BoolDeviceALMLog,
        BoolDeviceAlmFullLog,
        BoolDeviceALMActive1,
        StringDeviceALMPort1,
        IntDeviceALMLineCount1,
        BoolDeviceALMActive2,
        StringDeviceALMPort2,
        IntDeviceALMLineCount2,
        BoolDeviceALMActive3,
        StringDeviceALMPort3,
        IntDeviceALMLineCount3,
        BoolDeviceALMActive4,
        StringDeviceALMPort4,
        IntDeviceALMLineCount4,
        BoolDeviceALMBlockCallOut,
        StringDeviceALMBlockCallOutData,
        BoolDeviceALMBlockCallIn,
        StringDeviceALMBlockCallInData,

        #endregion

        #region Device Pana

        EnumStatusDevicePana,
        BoolDevicePanaEnable,
        BoolDevicePanaLog,
        BoolDevicePanaFullLog,
        IntDevicePanaType1,
        StringDevicePanaPort1,
        IntDevicePanaBaud1,
        IntDevicePanaDateFormat1,
        StringDevicePanaMapping1,
        StringDevicePanaInternal1,
        BoolDevicePanaIpexEnable,
        StringDevicePanaIpexAddress,
        IntDevicePanaIpexPort,
        BoolDevicePanaVirtualEnable,

        #endregion

        EnumStatusDeviceTelsa,

        EnumStatusDeviceVirtual,
        BoolDeviceVirtualEnable,
        BoolDeviceVirtualLog,


        EnumStatusPhoneMonitoring,


        EnumStatusMembership,


        BoolCustomColumnEnable1,
        StringCustomColumnTitle1,
        StringCustomColumnOid1,

        BoolCustomColumnEnable2,
        StringCustomColumnTitle2,
        StringCustomColumnOid2,

        BoolCustomColumnEnable3,
        StringCustomColumnTitle3,
        StringCustomColumnOid3,

        BoolCustomColumnEnable4,
        StringCustomColumnTitle4,
        StringCustomColumnOid4,

        BoolCustomColumnEnable5,
        StringCustomColumnTitle5,
        StringCustomColumnOid5,

        BoolCustomColumnEnable6,
        StringCustomColumnTitle6,
        StringCustomColumnOid6,

        BoolCustomColumnEnable7,
        StringCustomColumnTitle7,
        StringCustomColumnOid7,

        BoolCustomColumnEnable8,
        StringCustomColumnTitle8,
        StringCustomColumnOid8,

        BoolCustomColumnEnable9,
        StringCustomColumnTitle9,
        StringCustomColumnOid9,

        BoolCustomColumnEnable0,
        StringCustomColumnTitle0,
        StringCustomColumnOid0,
        EnumCallTeleCodeDisplayMode,
        EnumCallLineDisplayMode,
        EnumStatusMessaging,
        BoolDeviceTelsaEnable,
        BoolDeviceTelsaLog,
        BoolDeviceTelsaEnableRecord,
        BoolDeviceTelsaEnableVoiceMessage,
        StringDeviceTelsaServerName,
        StringDeviceTelsaServerPort,
        IntDeviceTelsaPriorityIndex1,
        IntDeviceTelsaPriorityIndex2,
        IntDeviceTelsaPriorityIndex3,
        StringDeviceTelsaVoiceMessageMapping,
        IntDeviceTelsaLineCount1,
        IntDeviceTelsaLineCount2,
        IntDeviceTelsaLineCount3,
        StringDeviceTelsaRecordPath,


        EnumStatusEmail,
        BoolEmailEnable,
        BoolEmailLog,
        IntEmailUpdateIndex,


        BoolWebEnable,
        StringWebAddress,


        BoolAutoUpdateEnable,
        StringAutoUpdateLastMessage,
        BoolAutoUpdateChecking,
        StringAutoUpdateNewVersion,
        BoolAutoUpdateDownloading,
        IntAutoUpdatePercent,
        BoolAutoUpdateReadyToInstall,
        BoolAutoUpdateForced,
        BoolAutoUpdateUseIEProxy,
        StringAutoUpdateFile,

        EnumStatusSMS,
        BoolSMSEnable,
        StringSMSNumber,
        StringSMSUserName,
        StringSMSPassword,
        StringSMSTestNumber,
        IntSMSScanDelay,
        StringSMSRemainCreadit,
        StringSMSLastMessage,
        BoolSMSLog,
        EnumSMSProvider,
        StringSMSURL,

        BoolSyncEnable,
        StringSyncServerUrl,
        IntSyncDurationIndex,

        StringAutoUpdateProxyUsername,
        StringAutoUpdateProxyPassword,
        StringAutoUpdateProxyPort,
        StringAutoUpdateProxyURL,
        BoolSMSDeliveryEnable,
        IntSyncCode,


        StringFirstName,
        StringLastName,
        StringCompany,
        StringPhoneNumber,
        StringAddress,
        StringIntCode,
        StringEcoCode,
        StringSarbargPath,


        StringDeviceALMRecordPath,
        StringDevicePanaRecordPath,

        StringinfoName,
        StringinfoCompany,
        StringinfoEmail,
        StringinfoPhone,
        StringinfoMobile,
        StringinfoAddress,


        BoolSmsFeedbackOnMissedCall,
        StringSmsFeedbackOnMissedCallNumber1,
        BoolSmsFeedbackOnNotWorkTime,
        StringSmsFeedbackOnNotWorkTimeNumber1,

        BoolSmsCallInAnswared,
        StringSmsCallInAnswaredText,
        BoolSmsCallInAnswaredAllways,
        BoolSmsCallInAnswaredPerday,
        IntSmsCallInAnswaredPerdayCount,

        BoolSmsCallInNotAnswared,
        StringSmsCallInNotAnswaredText,
        BoolSmsCallInNotAnswaredAllways,
        BoolSmsCallInNotAnswaredPerday,
        IntSmsCallInNotAnswaredPerdayCount,

        BoolSmsCallInNotWork,
        BoolSmsCallInNotWorkMorePriority,
        StringSmsCallInNotWorkText,
        BoolSmsCallInNotWorkAllways,
        BoolSmsCallInNotWorkPerday,
        IntSmsCallInNotWorkPerdayCount,
        StringSmsCallInNotWork1Start,
        StringSmsCallInNotWork1Stop,
        StringSmsCallInNotWork2Start,
        StringSmsCallInNotWork2Stop,
        StringSmsCallInNotWork3Start,
        StringSmsCallInNotWork3Stop,

        StringResetAdminPass,
        EnumCallExtDisplayMode,
        StringSmsDeviceList,


        BoolDevicePanaMultiExtEnable,
        StringDevicePanaMultiExtGroupCode,
        StringDevicePanaMultiExtCodes
    }
}
