using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumEmailAttType
    {
        [DataMember] Body,
        [DataMember] Inline,
        [DataMember] Attachment
    }
}
