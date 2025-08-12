using System;
using DevExpress.Xpo;
using POL.DB.Root;

namespace POL.DB.P30Office
{
    public struct ContactStruct
    {
        [Persistent]
        public Guid ContactOid { get; set; }

        [PersianString]
        [Size(128)]
        public string Title { get; set; }


        public override string ToString()
        {
            return Title;
        }
    }
}
