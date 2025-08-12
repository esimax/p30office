using System;

namespace POL.Lib.Interfaces
{
    public class SyncCheckResponse
    {
        public Guid SyncTableOid { get; set; }
        public Guid RecordOid { get; set; }
        public string ErrorMessage { get; set; }
    }
}
