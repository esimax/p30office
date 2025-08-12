using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public class CallStatHolder
    {
        [DataMember]
        public int CountryCode { get; set; }

        [DataMember]
        public int CityCode { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public int CountOut { get; set; }

        [DataMember]
        public int CountIn { get; set; }

        [DataMember]
        public int DurationOut { get; set; }

        [DataMember]
        public int DurationIn { get; set; }

        [DataMember]
        public EnumCallType LastCallType { get; set; }

        [DataMember]
        public DateTime LastCallDate { get; set; }

        [DataMember]
        public int LastDuration { get; set; }

        [DataMember]
        public int LastLine { get; set; }

        [DataMember]
        public int LastInternal { get; set; }

        [DataMember]
        public int MissCall { get; set; }
    }
}
