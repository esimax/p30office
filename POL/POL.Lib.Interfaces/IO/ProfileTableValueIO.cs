using System;

namespace POL.Lib.Interfaces.IO
{
    [Serializable]
    public class ProfileTableValueIO
    {
        public string Title { get; set; }
        public ProfileTableValueIO[] Values { get; set; }
    }
}
