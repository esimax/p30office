using System;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public class DecodedPhone
    {
        public DecodedPhone(string original)
        {
            OriginalData = original;
            CountryCode = string.Empty;
            CountryOid = Guid.Empty;

            CityCode = string.Empty;
            CityOid = Guid.Empty;

            Phone = string.Empty;

            ExtraDialed = string.Empty;

            HasError = false;
        }

        [DataMember]
        public string OriginalData { get; private set; }

        [DataMember]
        public string CountryCode { get; set; }

        [DataMember]
        public Guid CountryOid { get; set; }

        [DataMember]
        public string CityCode { get; set; }

        [DataMember]
        public Guid CityOid { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string ExtraDialed { get; set; }

        [DataMember]
        public bool HasError { get; set; }
    }
}
