using System;
using DevExpress.Xpo;
using POL.DB.Root;

namespace POL.DB.P30Office
{
    public struct FileStruct
    {
        [Persistent]
        [Size(256)]
        public string FileName { get; set; }

        public double Len { get; set; }

        [PersianString]
        public string Note { get; set; }

        [Persistent]
        public Guid ByteOid { get; set; }

        public override string ToString()
        {
            return FileName;
        }
    }
}
