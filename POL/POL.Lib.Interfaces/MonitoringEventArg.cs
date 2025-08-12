using System;

namespace POL.Lib.Interfaces
{
    public class MonitoringEventArg : EventArgs
    {
        public MonitoringEventArg(MonitoringItem item)
        {
            Item = item;
        }

        public MonitoringItem Item { get; set; }
    }
}
