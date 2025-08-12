using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumReportCallVerticalDataType
    {
        [DataMember] CallCount,
        [DataMember] CallDuration
    }
}
