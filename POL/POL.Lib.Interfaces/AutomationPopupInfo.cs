using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public class AutomationPopupInfo
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string PopupText { get; set; }

        [DataMember]
        public int PopupType { get; set; }

        [DataMember]
        public string PopupData { get; set; }

        [DataMember]
        public string CardTableCreatorTitle { get; set; }

        [DataMember]
        public Guid CardTableOid { get; set; }

        [DataMember]
        public EnumAutomationType AutomationType { get; set; }

        [DataMember]
        public string LinkCall { get; set; }

        [DataMember]
        public Guid LinkCallOid { get; set; }

        [DataMember]
        public string LinkEmail { get; set; }

        [DataMember]
        public Guid LinkEmailOid { get; set; }

        [DataMember]
        public string LinkSMS { get; set; }

        [DataMember]
        public Guid LinkSMSOid { get; set; }

        [DataMember]
        public string LinkContact { get; set; }

        [DataMember]
        public Guid LinkContactOid { get; set; }

        [DataMember]
        public string LinkCategory { get; set; }

        [DataMember]
        public Guid LinkCategoryOid { get; set; }
    }

    [DataContract]
    public enum EnumAutomationType
    {
        [EnumMember] Starting = 0,
        [EnumMember] Ending = 1,
        [EnumMember] Result = 2,
        [EnumMember] ProfileDateTime = 3
    }
}
