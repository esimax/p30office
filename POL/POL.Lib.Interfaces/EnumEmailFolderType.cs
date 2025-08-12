using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumEmailFolderType
    {
        [DataMember] Inbox,
        [DataMember] Sent,
        [DataMember] Trash,
        [DataMember] Drafts,
        [DataMember] Folder,
        [DataMember] WaitForSend
    }
}
