namespace POC.Module.Attachment.Models
{
    public class ImportProductStructure
    {
        public ImportProductStructure(int row)
        {
            Row = row;
            Column = 0;

            Code = 0;
            Title = string.Empty;
            Price = 0;
            Unit = string.Empty;
            
            ErrorType = EnumImportErrorType.None;
        }
        public int Column { get; set; }
        public int Row { get; set; }

        public int Code { get; set; }
        public string Title { get; set; }

        public decimal Price { get; set; }
        public string Unit { get; set; }
        
        public EnumImportErrorType ErrorType { get; set; }
    }
}
