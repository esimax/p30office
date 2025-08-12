using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumSMSDelivery
    {
        [DataMember] SendToStation,
        [DataMember] Success,
        [DataMember] Failed,
        [DataMember] None,
        [DataMember] NeedToCheck
    }
}
