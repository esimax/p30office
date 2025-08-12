using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumSelectionType
    {
        [DataMember] SelectedPhone,
        [DataMember] SelectedContact,
        [DataMember] SelectedContactList,
        [DataMember] SelectedCategory,
        [DataMember] SelectedBasket,
        [DataMember] CustomeSelection
    }
}
