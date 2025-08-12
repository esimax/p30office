using System;

namespace POL.Lib.Interfaces.PObjects
{
    [Serializable]
    public class Phone
    {
        public string PhoneNumber { get; set; }
        public EnumPhoneType PhoneType { get; set; }
        public string CountryTitle { get; set; }
        public string CountryCode { get; set; }

        public string CityTitle { get; set; }
        public string CityCode { get; set; }

        public string Title { get; set; }
        public string Note { get; set; }
    }
}
