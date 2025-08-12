using System.Runtime.Serialization;

namespace POL.Lib.Interfaces.Info
{
    [DataContract]
    public class InfoCPU
    {
        [DataMember]
        public string Caption { get; set; }

        [DataMember]
        public string Manufacturer { get; set; }

        [DataMember]
        public string MaxClockSpeed { get; set; }

        [DataMember]
        public string ProcessorID { get; set; }

        [DataMember]
        public string Socket { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string NumberOfCores { get; set; }
    }
}
