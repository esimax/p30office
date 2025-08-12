using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumAddressStatus
    {
        [DataMember] New,
        [DataMember] Survey,
        [DataMember] Old,
        [DataMember] Unkown
    }
}
