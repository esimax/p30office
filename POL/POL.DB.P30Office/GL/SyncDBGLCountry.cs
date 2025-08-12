using System;

namespace POL.DB.P30Office.GL
{
    public class SyncDBGLCountry
    {
        public Guid Oid { get; set; }
        public int Code_ID { get; set; }
        public string Code_FIPS104 { get; set; }
        public string ISO2 { get; set; }
        public string ISO3 { get; set; }
        public int ISON { get; set; }
        public string Internet { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public int TeleCode1 { get; set; }
        public int TeleCode2 { get; set; }
        public int TeleCode { get; set; }
        public string TitleEn { get; set; }
        public string TitleXX { get; set; }
        public byte[] FlagImage { get; set; }
        public byte[] MapImage { get; set; }
        public long TimeZone { get; set; }
    }
}
