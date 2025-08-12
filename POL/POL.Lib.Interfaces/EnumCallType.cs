using System.Runtime.Serialization;
using DevExpress.Xpo;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumCallType
    {
        [DataMember] [DisplayName("دریافتی")] CallIn,
        [DataMember] [DisplayName("ارسالی")] CallOut
    }
}
