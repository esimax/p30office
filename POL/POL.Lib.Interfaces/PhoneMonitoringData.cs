using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public class PhoneMonitoringData
    {
        public PhoneMonitoringData()
        {
            Status = EnumCallSystemLineStatus.Unkown;
        }

        [DataMember]
        public EnumCallSystemLineStatus Status { get; set; }


        [DataMember]
        public string OutContactTitle { get; set; }

        [DataMember]
        public string OutPhoneNumber { get; set; }

        [DataMember]
        public int OutPhoneCityCode { get; set; }

        [DataMember]
        public int OutPhoneCountryCode { get; set; }

        [DataMember]
        public string OutPhoneCityName { get; set; }

        [DataMember]
        public string OutPhoneCountryName { get; set; }

        [DataMember]
        public DateTime OutDateTime { get; set; }

        [DataMember]
        public TimeSpan OutDuration { get; set; }

        [DataMember]
        public bool OutTalking { get; set; }

        [DataMember]
        public string OutExt { get; set; }

        [DataMember]
        public string OutDialed { get; set; }


        [DataMember]
        public string InContactTitle { get; set; }

        [DataMember]
        public string InPhoneNumber { get; set; }

        [DataMember]
        public int InPhoneCityCode { get; set; }

        [DataMember]
        public int InPhoneCountryCode { get; set; }

        [DataMember]
        public string InPhoneCityName { get; set; }

        [DataMember]
        public string InPhoneCountryName { get; set; }

        [DataMember]
        public DateTime InDateTime { get; set; }

        [DataMember]
        public TimeSpan InDuration { get; set; }

        [DataMember]
        public bool InTalking { get; set; }

        [DataMember]
        public string InExt { get; set; }

        [DataMember]
        public DateTime InLastRingDate { get; set; }
    }
}
