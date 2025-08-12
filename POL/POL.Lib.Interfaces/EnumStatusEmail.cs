using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumStatusEmail
    {
        [EnumMember] Undefined,
        [EnumMember] Checking,
        [EnumMember] OK,

        [EnumMember] InvalidEmailSettings,
        [EnumMember] FailedLoginToEmailServer
    }
}
