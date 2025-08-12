using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public class MessagingItem
    {
        [DataMember]
        public Guid Oid { get; set; }

        [DataMember]
        public DateTime MessageDate { get; set; }

        [DataMember]
        public EnumMessageKind MessageKind { get; set; }

        [DataMember]
        public EnumPermissionBaseType PermissionBaseType { get; set; }

        [DataMember]
        public string PermissionValue { get; set; }

        [DataMember]
        public string[] MessageData { get; set; }

        [DataMember]
        public Guid From { get; set; }

        [DataMember]
        public Guid To { get; set; }
    }
}
