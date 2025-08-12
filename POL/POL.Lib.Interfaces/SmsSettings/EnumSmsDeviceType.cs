using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces.SmsSettings
{
    [Serializable]
    [DataContract]
    public enum EnumSmsDeviceType
    {
        [DataMember] Software,
        [DataMember] Hardware
    }
}
