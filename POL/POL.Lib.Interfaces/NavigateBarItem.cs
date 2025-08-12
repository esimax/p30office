using System;

namespace POL.Lib.Interfaces
{
    public class NavigateBarItem
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Tooltip { get; set; }
        public int Order { get; set; }
        public int AccessLevel { get; set; }
        public object Content { get; set; }
        public bool IsFirst { get; set; }
        public Type MainViewType { get; set; }
        public Type MainModuleType { get; set; }
    }
}
