using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumStatusSMS
    {
        [EnumMember] Undefined,
        [EnumMember] Checking,
        [EnumMember] OK,

        [EnumMember] InvalidSMSConnection,
        [EnumMember] InvalidSMSSettings
    }
}
