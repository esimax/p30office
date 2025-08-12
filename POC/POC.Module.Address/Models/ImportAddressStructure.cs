using System;

namespace POC.Module.Address.Models
{
    public class ImportAddressStructure
    {
        public ImportAddressStructure(int row)
        {
            Row = row;
            Column = 0;

            ContactCode = 0;
            ContactTitle = string.Empty;

            Title = string.Empty;
            City = string.Empty;
            Area = string.Empty;
            Address = string.Empty;
            POBox = string.Empty;
            ZipCode = string.Empty;
            Note = string.Empty;

            ErrorType = EnumImportErrorType.None;
        }
        public int Column { get; set; }
        public int Row { get; set; }

        public int ContactCode { get; set; }
        public string ContactTitle { get; set; }

        public string Title { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string Address { get; set; }
        public string POBox { get; set; }
        public string ZipCode { get; set; }
        public string Note { get; set; }

        public EnumImportErrorType ErrorType { get; set; }

        public Guid CountryOid { get; set; }
        public Guid CityOid { get; set; }
    }
}
