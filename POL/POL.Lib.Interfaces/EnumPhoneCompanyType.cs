using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumPhoneCompanyType
    {
        [DataMember] Unknown,
        [DataMember] HamrahAval,
        [DataMember] IranCell,
        [DataMember] RighTel,
        [DataMember] Talia
    }
}
