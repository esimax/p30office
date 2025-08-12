using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumStatusDevicePana
    {
        [EnumMember] Undefined,
        [EnumMember] Checking,
        [EnumMember] OK,
        [EnumMember] Disabled,
        [EnumMember] Confilict,
        [EnumMember] InvalidPort,
        [EnumMember] InvalidSettings,
        [EnumMember] InvalidDevic
    }
}
