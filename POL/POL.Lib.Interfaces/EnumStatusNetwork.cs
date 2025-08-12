using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumStatusNetwork
    {
        [EnumMember] Undefined,
        [EnumMember] Checking,
        [EnumMember] OK,
        [EnumMember] InvalidHost,

        [EnumMember] InvalidAddress,
        [EnumMember] InvalidPort,

        [EnumMember] InvalidServerInformation
    }
}
