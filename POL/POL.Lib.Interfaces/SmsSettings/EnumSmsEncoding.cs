using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces.SmsSettings
{
    [Serializable]
    [DataContract]
    public enum EnumSmsEncoding
    {
        [DataMember] Auto,
        [DataMember] Ansi,
        [DataMember] Unicode
    }
}
