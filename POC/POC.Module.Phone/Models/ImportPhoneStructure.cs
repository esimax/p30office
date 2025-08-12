using System;

namespace POC.Module.Phone.Models
{
    public class ImportPhoneStructure
    {
        public ImportPhoneStructure(int row)
        {
            Row = row;
            Column = 0;

            ContactCode = 0;
            ContactTitle = string.Empty;

            CountryCode = string.Empty;
            CityCode = string.Empty;
            PhoneNumber = string.Empty;
            PhoneTitle = string.Empty;
            PhoneNote = string.Empty;

            ErrorType = EnumImportErrorType.None;
        }
        public int Column { get; set; }
        public int Row { get; set; }

        public int ContactCode { get; set; }
        public string ContactTitle { get; set; }

        public string CountryCode { get; set; }
        public string CityCode { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneTitle { get; set; }
        public string PhoneNote { get; set; }

        public EnumImportErrorType ErrorType { get; set; }

        public Guid CountryOid { get; set; }
        public Guid CityOid { get; set; }
    }
}
