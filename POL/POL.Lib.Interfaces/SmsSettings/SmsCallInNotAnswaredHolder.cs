using System;

namespace POL.Lib.Interfaces.SmsSettings
{
    public class SmsCallInNotAnswaredHolder
    {
        public int Count { get; set; }
        public DateTime Date { get; set; }
        public string PhoneNumber { get; set; }
        public int CountryCode { get; set; }
        public int CityCode { get; set; }
    }
}
