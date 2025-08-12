using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumCallSystemLineStatus
    {
        [EnumMember] Unkown = 0,
        [EnumMember] Ring = 1,
        [EnumMember] HookOn = 2,
        [EnumMember] HookOff = 3,
        [EnumMember] CallerID = 4,
        [EnumMember] Dialed = 5,
        [EnumMember] Trans = 6
    }
}
