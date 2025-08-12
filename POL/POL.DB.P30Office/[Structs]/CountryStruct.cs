using System;
using DevExpress.Xpo;

namespace POL.DB.P30Office
{
    public struct CountryStruct
    {
        [Persistent]
        public Guid CountOid { get; set; }

        public string CountTitle { get; set; }

        public override string ToString()
        {
            return CountTitle;
        }
    }
}
