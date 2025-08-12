using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumByteDataType
    {
        [DataMember] ProfileImage,
        [DataMember] ProfileFile,
        [DataMember] ProfileMap,

        [DataMember] ListImage,
        [DataMember] ListFile,
        [DataMember] ListMap
    }
}
