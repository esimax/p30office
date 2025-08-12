using System;

namespace POL.Lib.Interfaces
{
    public class StatisticsEventArg : EventArgs
    {
        public StatisticsEventArg(StatisticsItem item)
        {
            Item = item;
        }

        public StatisticsItem Item { get; set; }
    }
}
