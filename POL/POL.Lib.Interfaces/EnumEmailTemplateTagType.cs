using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumEmailTemplateTagType
    {
        [DataMember] String,
        [DataMember] Memo,
        [DataMember] ProfileItem
    }
}
