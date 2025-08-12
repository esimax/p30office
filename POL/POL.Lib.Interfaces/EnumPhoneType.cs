using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumPhoneType
    {
        [DataMember] PhoneFax,
        [DataMember] Mobile,
        [DataMember] SMS
    }
}
