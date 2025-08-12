using System;
using System.Collections.Generic;

namespace POL.Lib.Interfaces.PObjects
{
    [Serializable]
    public class ProfileRoot
    {
        public ProfileRoot()
        {
            Groups = new List<ProfileGroup>();
        }

        public string Title { get; set; }
        public List<ProfileGroup> Groups { get; set; }
    }
}
