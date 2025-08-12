using System;

namespace POL.Lib.Interfaces.IO
{
    [Serializable]
    public class PackIOProfileItem
    {
        public string Title { get; set; }
        public int Order { get; set; }
        public EnumProfileItemType ItemType { get; set; }

        public Guid Guid1 { get; set; }

        public int Int1 { get; set; }
        public int Int2 { get; set; }
        public int Int3 { get; set; }

        public double Double1 { get; set; }
        public double Double2 { get; set; }

        public string String1 { get; set; }
        public string String2 { get; set; }
        public string String3 { get; set; }

        public string TableName { get; set; }

        public string UnicCode { get; set; }
    }
}
