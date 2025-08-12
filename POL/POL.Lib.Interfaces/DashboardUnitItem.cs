using System;

namespace POL.Lib.Interfaces
{
    public class DashboardUnitItem
    {
        public string Key { get; set; }
        public string TabName { get; set; }
        public int Order { get; set; }
        public int Permission { get; set; }
        public Type ContentType { get; set; }
    }
}
