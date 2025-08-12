using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces.SmsSettings
{
    [Serializable]
    [DataContract]
    public enum EnumSmsLongMessages
    {
        [DataMember] Concatenate,
        [DataMember] Turncate,
        [DataMember] SimpleSplit,
        [DataMember] FormatedSplit
    }
}
