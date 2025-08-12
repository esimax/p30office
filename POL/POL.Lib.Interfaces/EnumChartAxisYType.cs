using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumChartAxisYType
    {
        [DataMember] CallCount,
        [DataMember] CallDuration
    }
}
