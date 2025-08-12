using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces.SmsSettings
{
    [Serializable]
    [DataContract]
    public class SmsMultiModuleSettings
    {
        [DataMember]
        public SmsModuleSettings[] Settings { get; set; }
    }
}
