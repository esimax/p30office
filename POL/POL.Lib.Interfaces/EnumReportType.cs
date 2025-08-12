using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumReportType
    {
        [DataMember] BarChart,
        [DataMember] PieChart,
        [DataMember] Grid
    }
}
