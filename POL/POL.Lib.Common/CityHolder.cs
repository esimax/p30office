using System;

namespace POL.Lib.Common
{
    public class CityHolder
    {
        public CityHolder()
        {
            CityCode = string.Empty;
            CityOid = Guid.Empty;
            CityPhoneLen = 0;
        }

        public Guid CityOid { get; set; }
        public string CityCode { get; set; }
        public int CityPhoneLen { get; set; }
    }
}
