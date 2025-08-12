using System;

namespace POC.Module.Link.Models
{
    [Serializable]
    public class LinkRelMainIO
    {
        public LinkRelSubIO[] Subs { get; set; }
    }
}
