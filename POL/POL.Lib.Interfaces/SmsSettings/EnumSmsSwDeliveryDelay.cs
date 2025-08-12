using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces.SmsSettings
{
    [Serializable]
    [DataContract]
    public enum EnumSmsSwDeliveryDelay
    {
        [DataMember] OneMiniut,
        [DataMember] TenMiniut,
        [DataMember] OneHour,
        [DataMember] SixHour,
        [DataMember] OneDay
    }
}
