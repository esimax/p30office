using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public class ContactInfo
    {
        public ContactInfo()
        {
            Oid = Guid.Empty;
            Title = string.Empty;
            Code = -1;
        }

        [DataMember]
        public Guid Oid { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public int Code { get; set; }

        [DataMember]
        public string Cats { get; set; }

        [DataMember]
        public string CCText1 { get; set; }

        [DataMember]
        public string CCText2 { get; set; }

        [DataMember]
        public string CCText3 { get; set; }

        [DataMember]
        public string CCText4 { get; set; }

        [DataMember]
        public string CCText5 { get; set; }

        [DataMember]
        public string CCText6 { get; set; }

        [DataMember]
        public string CCText7 { get; set; }

        [DataMember]
        public string CCText8 { get; set; }

        [DataMember]
        public string CCText9 { get; set; }

        [DataMember]
        public string CCText0 { get; set; }

        [DataMember]
        public string Creator { get; set; }

        [DataMember]
        public string Editor { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public DateTime EditedDate { get; set; }
    }
}
