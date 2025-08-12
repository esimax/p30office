using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumStatusMembership
    {
        [EnumMember] Undefined,
        [EnumMember] Checking,
        [EnumMember] OK,

        [EnumMember] InvalidAdminCreation,

        [EnumMember] InvalidHostAddress,
        [EnumMember] InvalidHostPort,
        [EnumMember] InvalidHost,
        [EnumMember] InvalidHostUnexpected
    }
}
