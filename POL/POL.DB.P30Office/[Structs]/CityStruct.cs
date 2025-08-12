using System;
using DevExpress.Xpo;

namespace POL.DB.P30Office
{
    public struct CityStruct
    {
        [Persistent]
        public Guid CityOid { get; set; }

        public string CityTitle { get; set; }

        public override string ToString()
        {
            return CityTitle;
        }
    }
}
