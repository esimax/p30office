using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumPermissionBaseType
    {
        [EnumMember] NoOne,
        [EnumMember] EveryOne,
        [EnumMember] RoleBase,
        [EnumMember] UserBase
    }
}
