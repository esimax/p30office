using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public class SMSInfo
    {
        public SMSInfo()
        {
            From = string.Empty;
            To = string.Empty;
            Count = 0;
            Text = string.Empty;
            Oid = Guid.Empty;
        }

        [DataMember]
        public Guid Oid { get; set; }

        [DataMember]
        public string From { get; set; }

        [DataMember]
        public string To { get; set; }

        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string Sender { get; set; }

        [DataMember]
        public string Result { get; set; }

        [DataMember]
        public string DelivaryResult { get; set; }

        [DataMember]
        public int ViewPermissionType { get; set; }

        [DataMember]
        public Guid ViewOid { get; set; }
    }
}
