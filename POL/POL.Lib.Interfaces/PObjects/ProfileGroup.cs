using System;
using System.Collections.Generic;

namespace POL.Lib.Interfaces.PObjects
{
    [Serializable]
    public class ProfileGroup
    {
        public ProfileGroup()
        {
            Items = new List<ProfileItems>();
        }

        public string Title { get; set; }
        public List<ProfileItems> Items { get; set; }
    }
}
