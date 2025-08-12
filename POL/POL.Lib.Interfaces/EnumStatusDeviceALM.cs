using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumStatusDeviceALM
    {
        [EnumMember] Undefined,
        [EnumMember] Checking,
        [EnumMember] OK,
        [EnumMember] Disabled,
        [EnumMember] Confilict,


        [EnumMember] InvalidCurrentCity,
        [EnumMember] InvalidCurrentCityNotFound,
        [EnumMember] InvalidDevic1,
        [EnumMember] InvalidDevic2,
        [EnumMember] InvalidDevic3,
        [EnumMember] InvalidDevic4
    }
}
