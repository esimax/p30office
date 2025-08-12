using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumMessageKind
    {
        [EnumMember] CallerID,
        [EnumMember] CallTrans,
        [EnumMember] CallEnd,
        [EnumMember] MissCall,
        [EnumMember] ShoutDown,
        [EnumMember] EmailSend,
        [EnumMember] EmailReceive,
        [EnumMember] SMSSendSuccess,
        [EnumMember] SMSReceive,
        [EnumMember] Fax,
        [EnumMember] Chat,
        [EnumMember] TaskAlert,
        [EnumMember] VoiceMessage,
        [EnumMember] CacheChanged,

        [EnumMember] ApplicationUpdate,

        [EnumMember] BlackList,

        [EnumMember] Support,


        [EnumMember] SMSSendFailed,
        [EnumMember] SMSForwardOnCredit,
        [EnumMember] SMSForwardOnFailed,
        [EnumMember] SMSWaitForDelivery,
        [EnumMember] SMSDeliveryResult,

        [EnumMember] Backup,

        [EnumMember] DriveStorage
    }
}
