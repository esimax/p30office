using System;

namespace POL.Lib.Interfaces
{
    [Serializable]
    public class LicenceRequestData
    {
        public int AgentCode { get; set; }
        public string AppName { get; set; }
        public string Calendar { get; set; }
        public string Date { get; set; }
        public int LicenseType { get; set; }
        public string OwnerAddress { get; set; }
        public string OwnerCompany { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerMobile { get; set; }
        public string OwnerPhone { get; set; }
        public string Serial { get; set; }
        public string Version { get; set; }
        public string PublicKay { get; set; }
    }
}
