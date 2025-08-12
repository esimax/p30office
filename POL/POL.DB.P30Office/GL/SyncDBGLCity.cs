using System;

namespace POL.DB.P30Office.GL
{
    public class SyncDBGLCity
    {
        public Guid Oid { get; set; }
        public string TitleEN { get; set; }
        public string TitleXX { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long TimeZone { get; set; }
        public int PhoneCode { get; set; }
        public bool HasTeleCode { get; set; }
        public string StateTitle { get; set; }
        public int PhoneLen { get; set; }
        public Guid CountryOid { get; set; }
    }
}
