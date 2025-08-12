using System.Runtime.Serialization;

namespace POL.Lib.Interfaces.Info
{
    [DataContract]
    public class InfoLogicalDisk
    {
        [DataMember]
        public string Caption { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string FileSystem { get; set; }

        [DataMember]
        public string Size { get; set; }

        [DataMember]
        public string FreeSpace { get; set; }

        [DataMember]
        public string VolumeSerialNumber { get; set; }

        [DataMember]
        public string VolumeName { get; set; }
    }
}
