using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumContactSelectType
    {
        [DataMember] All,
        [DataMember] Category,
        [DataMember] SelectionBasket
    }
}
