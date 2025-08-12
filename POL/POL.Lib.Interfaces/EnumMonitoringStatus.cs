using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumMonitoringStatus
    {
        [DataMember] Undefined,
        [DataMember] OK,
        [DataMember] Warning,
        [DataMember] Error,
        [DataMember] Duty
    }
}
