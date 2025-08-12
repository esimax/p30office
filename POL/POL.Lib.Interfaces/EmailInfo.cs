using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public class EmailInfo
    {
        public EmailInfo()
        {
            From = string.Empty;
            To = string.Empty;
            Subject = string.Empty;
            Oid = Guid.Empty;
        }

        [DataMember]
        public Guid Oid { get; set; }

        [DataMember]
        public string From { get; set; }

        [DataMember]
        public string To { get; set; }

        [DataMember]
        public string Subject { get; set; }

        [DataMember]
        public int ViewPermissionType { get; set; }

        [DataMember]
        public Guid ViewOid { get; set; }
    }
}
