using System;

namespace POL.Lib.Interfaces
{
    public class POSClientInformation
    {
        public POSClientInformation()
        {
            ConnectionDate = DateTime.MinValue;
            LoginDate = DateTime.MinValue;
            LastReregisterdDate = DateTime.MinValue;

            UserName = string.Empty;
        }

        public ClientToServerInformation CTSI { get; set; }
        public DateTime ConnectionDate { set; get; }


        public string UserName { get; set; }
        public DateTime LoginDate { set; get; }
        public DateTime LastReregisterdDate { set; get; }
    }
}
