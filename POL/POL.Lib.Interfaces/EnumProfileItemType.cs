using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumProfileItemType
    {
        [DataMember] Bool,
        [DataMember] Double,
        [DataMember] Country,
        [DataMember] City,
        [DataMember] Location,
        [DataMember] String,
        [DataMember] Memo,
        [DataMember] StringCombo,
        [DataMember] StringCheckList,
        [DataMember] Color,
        [DataMember] File,
        [DataMember] Image,
        [DataMember] Date,
        [DataMember] Time,
        [DataMember] DateTime,
        [DataMember] Contact,
        [DataMember] List
    }
}
