using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumStatusMessaging
    {
        [EnumMember] Undefined,
        [EnumMember] Checking,
        [EnumMember] OK,

        [EnumMember] InvalidHostAddress,
        [EnumMember] InvalidHostPort,
        [EnumMember] InvalidHost,
        [EnumMember] InvalidHostUnexpected
    }
}
