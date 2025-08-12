using System;

namespace POL.Lib.Interfaces.IO
{
    [Serializable]
    public class PackIOProfileGroup
    {
        public string Title { get; set; }
        public int Order { get; set; }
        public PackIOProfileItem[] Items { get; set; }
    }
}
