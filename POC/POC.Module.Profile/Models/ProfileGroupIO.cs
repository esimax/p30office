using System;

namespace POC.Module.Profile.Models
{
    [Serializable]
    public  class PackIOProfileGroup
    {
        public string Title { get; set; }
        public int Order { get; set; }
        public PackIOProfileItem[] Items { get; set; }
    }
}
