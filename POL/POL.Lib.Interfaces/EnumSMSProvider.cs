using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumSMSProvider
    {
        [DataMember] XOfficeCom,
        [DataMember] SmsIr,
        [DataMember] FaraPayamakCom,
        [DataMember] IranTc,
        [DataMember] NeginsCom,
        [DataMember] SmsOnline,
        [DataMember] IranTc21,
        [DataMember] Afe,
        [DataMember] AZAD,
        [DataMember] iSMS,
        [DataMember] SunWay
    }
}
