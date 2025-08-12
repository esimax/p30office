using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public class CallInfo
    {
        public CallInfo()
        {
            Device = -1;
            Line = -1;
            Oid = Guid.Empty;
            IsCallIn = false;
            CallDate = DateTime.MinValue;
            CountryCode = -1;
            CountryTitle = string.Empty;
            CityCode = -1;
            CityTitle = string.Empty;
            PhoneTitle = string.Empty;
            Phone = string.Empty;
            PhoneInternal = string.Empty;
            Extra = string.Empty;
            Duration = -1;

            LastExt = -1;
            PrevExt = -1;
            LastExtDuration = -1;
        }


        [DataMember]
        public int Device { get; set; }

        [DataMember]
        public int Line { get; set; }

        [DataMember]
        public Guid Oid { get; set; }

        [DataMember]
        public bool IsCallIn { get; set; }


        [DataMember]
        public DateTime CallDate { get; set; }

        [DataMember]
        public int CountryCode { get; set; }

        [DataMember]
        public int CityCode { get; set; }

        [DataMember]
        public string CountryTitle { get; set; }

        [DataMember]
        public string CityTitle { get; set; }

        [DataMember]
        public string PhoneTitle { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string PhoneInternal { get; set; }

        [DataMember]
        public string Extra { get; set; }

        [DataMember]
        public int Duration { get; set; }


        [DataMember]
        public int LastExt { get; set; }

        [DataMember]
        public int PrevExt { get; set; }

        [DataMember]
        public int LastExtDuration { get; set; }

        [DataMember]
        public bool IsTrans { get; set; }


        [DataMember]
        public CallStatHolder CallStat { get; set; }

        [DataMember]
        public CallStatHolder TodayCallStat { get; set; }


        [DataMember]
        public string LastNote { get; set; }


        [DataMember]
        public int ExtraCode { get; set; }

        [DataMember]
        public string ExtraCodeTitle { get; set; }
    }
}
