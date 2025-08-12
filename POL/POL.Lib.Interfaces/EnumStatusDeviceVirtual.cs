using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    [Obsolete]
    public enum EnumStatusDeviceVirtual
    {
        [EnumMember] Undefined,
        [EnumMember] Checking,
        [EnumMember] OK,
        [EnumMember] Disabled,
        [EnumMember] Confilict,

        [EnumMember] InvalidCurrentCity,
        [EnumMember] InvalidCurrentCityNotFound
    }
}
