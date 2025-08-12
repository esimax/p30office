using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumStatusTelecommunication
    {
        [EnumMember] Undefined,
        [EnumMember] Checking,
        [EnumMember] OK,
        [EnumMember] Invalid
    }
}
