using System;

namespace POL.Lib.Interfaces.Info
{
    [Serializable]
    public class InfoAll
    {
        public InfoOS OS { get; set; }

        public InfoCPU CPU { get; set; }

        public InfoLogicalDisk[] LogicalDisks { get; set; }

        public InfoSystem System { get; set; }

        public InfoNetwork[] Networks { get; set; }
    }
}
