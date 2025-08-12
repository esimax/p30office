using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumKeyboardLayout
    {
        [DataMember] Default,
        [DataMember] RTL,
        [DataMember] LTR
    }
}
