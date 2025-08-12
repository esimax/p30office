using System;

namespace POL.Lib.Interfaces.IO
{
    [Serializable]
    public class ProfileTableNameIO
    {
        public string TableTitle { get; set; }
        public int ValueDepth { get; set; }
        public ProfileTableValueIO[] Values { get; set; }
    }
}
