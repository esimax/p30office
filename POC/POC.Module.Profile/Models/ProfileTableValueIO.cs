using System;

namespace POC.Module.Profile.Models
{
    [Serializable]
    public class ProfileTableValueIO
    {
        public string Title { get; set; }
        public ProfileTableValueIO[] Values { get; set; }
    }
}
