using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumCallLineDisplayMode
    {
        [DataMember] Code,
        [DataMember] Name,
        [DataMember] Both
    }
}
