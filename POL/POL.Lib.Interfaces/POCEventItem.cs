using System.Collections.Generic;

namespace POL.Lib.Interfaces
{
    public class POCEventItem
    {
        public POCEventItem()
        {
            Parameters = new List<POCEventParameter>();
        }

        public string Key { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }

        public List<POCEventParameter> Parameters { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
