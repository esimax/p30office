using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumShareStatus
    {
        [DataMember] None,
        [DataMember] SharedForOthers,
        [DataMember] OthersSharedThis
    }
}
