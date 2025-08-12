using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumCachDataType
    {
        [DataMember] ProfileItem,
        [DataMember] Roles,
        [DataMember] ContactCat,
        [DataMember] Country,
        [DataMember] ProfileTable
    }
}
