using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumReportCallHorizontalDataType
    {
        [DataMember] DateTime,
        [DataMember] DayOfWeek,
        [DataMember] Line,
        [DataMember] Country,
        [DataMember] State,
        [DataMember] City
    }
}
