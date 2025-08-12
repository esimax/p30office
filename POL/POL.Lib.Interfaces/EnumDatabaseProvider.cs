using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumDatabaseProvider
    {
        [EnumMember] Undefined,
        [EnumMember] MSSQL,
        [EnumMember] Access,
        [EnumMember] MSSQLCE,
        [EnumMember] XML
    }
}
