using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumMembershipStatus
    {
        [DataMember] InvalidNetwork,
        [DataMember] AccessDenide,
        [DataMember] BeforLogin,
        [DataMember] AfterLogin,
        [DataMember] BeforeLogout,
        [DataMember] AfterLogout
    }
}
