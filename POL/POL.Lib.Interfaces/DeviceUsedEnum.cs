using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumDeviceUsed
    {
        [EnumMember] None,
        [EnumMember] ALM,
        [EnumMember] Panasonic,
        [EnumMember] [Obsolete] Virtual,
        [EnumMember] Telsa,
        [EnumMember] Collision
    }
}
