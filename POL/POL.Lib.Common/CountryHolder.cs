using System;

namespace POL.Lib.Common
{
    public class CountryHolder
    {
        public CountryHolder()
        {
            CountryOid = Guid.Empty;
            CountryCode = string.Empty;
        }

        public Guid CountryOid { get; set; }
        public string CountryCode { get; set; }
    }
}
