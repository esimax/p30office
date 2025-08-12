using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumStatusDeviceTelsa
    {
        [EnumMember] Undefined,
        [EnumMember] Checking,
        [EnumMember] OK,
        [EnumMember] Disabled,
        [EnumMember] Confilict,


        [EnumMember] InvalidCurrentCity,
        [EnumMember] InvalidCurrentCityNotFound,
        [EnumMember] InvalidDevic,


        [EnumMember] InvalidHostAddress,
        [EnumMember] InvalidHostPort,
        [EnumMember] InvalidHost,
        [EnumMember] InvalidHostUnexpected
    }
}
