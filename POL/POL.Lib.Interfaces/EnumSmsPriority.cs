using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumSmsPriority
    {
        [DataMember] Low, 
        [DataMember] Normal, 
        [DataMember] High 
    }
}
