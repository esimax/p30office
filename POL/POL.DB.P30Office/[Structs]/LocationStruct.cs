using System;
using DevExpress.Xpo;
using POL.DB.Root;

namespace POL.DB.P30Office
{
    public struct LocationStruct
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int Zoom { get; set; }

        [PersianString]
        public string Note { get; set; }

        [Persistent]
        public Guid ByteOid { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Lat.ToString("n2"), Lon.ToString("n2"));
        }
    }
}
