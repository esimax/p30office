using System.Runtime.Serialization;

namespace POL.Lib.Interfaces.Info
{
    [DataContract]
    public class InfoNetwork
    {
        [DataMember]
        public string AdapterType { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string MACAddress { get; set; }

        [DataMember]
        public string Manufacturer { get; set; }

        [DataMember]
        public bool PhysicalAdapter { get; set; }

        [DataMember]
        public string Speed { get; set; }
    }
}
