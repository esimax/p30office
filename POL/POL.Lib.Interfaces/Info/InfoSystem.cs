using System.Runtime.Serialization;

namespace POL.Lib.Interfaces.Info
{
    [DataContract]
    public class InfoSystem
    {
        [DataMember]
        public string Manufacturer { get; set; }

        [DataMember]
        public string Model { get; set; }

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public string[] Roles { get; set; }
    }
}
