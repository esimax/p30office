using System;
using DevExpress.Xpo;
using POL.DB.Root;

namespace POL.DB.P30Office
{
    public struct ImageStruct
    {
        [Persistent]
        public int Width { get; set; }

        [Persistent]
        public int Height { get; set; }

        [PersianString]
        public string Note { get; set; }

        [Persistent]
        public Guid ByteOid { get; set; }

        public override string ToString()
        {
            return Note;
        }
    }
}
