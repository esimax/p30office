using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumEmailAutoDeletePolicy
    {
        [DataMember] DoNotDelete,
        [DataMember] DeleteAllFromCount,
        [DataMember] DeleteAllFromDay
    }
}
