using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumStatusDatabase
    {
        [EnumMember] Undefined,
        [EnumMember] Checking,
        [EnumMember] OK,
        [EnumMember] Invalid,
        [EnumMember] InvalidProvider,
        [EnumMember] InvalidThread
    }
}
