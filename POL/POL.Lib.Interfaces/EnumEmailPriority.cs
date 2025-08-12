using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumEmailPriority
    {
        [DataMember] Highest = 1,
        [DataMember] High = 2,
        [DataMember] Normal = 3,
        [DataMember] Low = 4,
        [DataMember] Lowest = 5
    }
}
