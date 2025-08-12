namespace POL.Lib.Interfaces
{
    public class XOStat
    {
        public string Serial { get; set; }
        public string Company { get; set; }
        public string Owner { get; set; }
        public string Version { get; set; }
        public int LU { get; set; }
        public int LC { get; set; }
        public long LD { get; set; }


        public long StatDate { get; set; }
        public string XOtype { get; set; }

        public int UserCount { get; set; }
        public int RoleCount { get; set; }
        public int CategoryCount { get; set; }
        public int ContactCount { get; set; }
        public int PhoneBook { get; set; }
        public int ProfileCount { get; set; }
        public int CallInCount { get; set; }
        public int CallOutCount { get; set; }
        public int EmailAccCount { get; set; }
        public int SMSInCount { get; set; }
        public int SMSOutCount { get; set; }
        public int CardTableCount { get; set; }
        public int AutomationCount { get; set; }

        public bool Tamas { get; set; }
        public string infoName { get; set; }
        public string infoCompany { get; set; }
        public string infoEmail { get; set; }
        public string infoMobile { get; set; }
        public string infoPhone { get; set; }
        public string infoAddress { get; set; }
    }
}
