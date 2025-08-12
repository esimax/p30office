using System;

namespace POL.Lib.Interfaces
{
    public class ApplicationBarHolder
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public Type ViewType { get; set; }
        public Type ModelType { get; set; }

        public bool IsFirst { get; set; }
    }
}
