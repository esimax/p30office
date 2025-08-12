using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumStatusLicense
    {
        [EnumMember] Undefined,
        [EnumMember] Checking,
        [EnumMember] OK,
        [EnumMember] Invalid,
        [EnumMember] RequiresUpdateC,
        [EnumMember] RequiresUpdateV,
        [EnumMember] Corrupted,
        [EnumMember] Expired,
        [EnumMember] OutofVersion
    }
}
