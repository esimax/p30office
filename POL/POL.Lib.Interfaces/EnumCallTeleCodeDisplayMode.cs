using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumCallTeleCodeDisplayMode
    {
        [DataMember] CodeAll,
        [DataMember] CodeCity,
        [DataMember] CodeCountry,

        [DataMember] NameCity,
        [DataMember] NameCountry,
        [DataMember] NameAll,

        [DataMember] CodeNameCity,
        [DataMember] CodeNameCountry,
        [DataMember] CodeNameAll
    }
}
