using System;

namespace POL.Lib.Interfaces
{
    public class ContactSelectByResult
    {
        public EnumContactSelectType SelectionType { get; set; }
        public Guid SelectionOid { get; set; }
    }
}
