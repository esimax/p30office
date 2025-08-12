using System;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace POL.Lib.Interfaces
{
    public class MetaDataProfileReportItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid ProfileItemOid { get; set; }
        public string Settings { get; set; }
        public int Order { get; set; }

        [XmlIgnore]
        public object ProfileItem { get; set; }

        [XmlIgnore]
        public BitmapSource Image { get; set; }
    }
}
