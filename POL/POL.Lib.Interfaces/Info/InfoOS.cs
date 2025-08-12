using System.Runtime.Serialization;

namespace POL.Lib.Interfaces.Info
{
    [DataContract]
    public class InfoOS
    {
        [DataMember]
        public string Caption { get; set; }

        [DataMember]
        public string ServicePack { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string Architecture { get; set; }

        [DataMember]
        public string Serial { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string ComputerName { get; set; }

        [DataMember]
        public string UserName { get; set; }
    }
}
