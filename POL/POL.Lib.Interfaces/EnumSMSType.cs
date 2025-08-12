using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [Obsolete]
    [DataContract]
    public enum EnumSMSType
    {
        [DataMember] Receive, 
        [DataMember] RequestToSend, 
        [DataMember] Send, 
        [DataMember] InQueue 
    }


    [DataContract]
    public enum EnumSmsType
    {
        [DataMember] Receive, 
        [DataMember] RequestToSend, 
        [DataMember] SendDone 
    }
}
