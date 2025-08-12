namespace POC.Module.Email.Models
{
    public class ImportEmailStructure
    {
        public ImportEmailStructure(int row)
        {
            Row = row;
            Column = 0;

            ContactCode = 0;
            ContactTitle = string.Empty;

            Title = string.Empty;
            Address = string.Empty;
            
            ErrorType = EnumImportErrorType.None;
        }
        public int Column { get; set; }
        public int Row { get; set; }

        public int ContactCode { get; set; }
        public string ContactTitle { get; set; }

        public string Title { get; set; }
        public string Address { get; set; }

        public EnumImportErrorType ErrorType { get; set; }
    }
}
