using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumCalendarType
    {
        [DataMember] ApplicationSettings,
        [DataMember] Hijri,
        [DataMember] Gregorian,
        [DataMember] Shamsi
    }
}
