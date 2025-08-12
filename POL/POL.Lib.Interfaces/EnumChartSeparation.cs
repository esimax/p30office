using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumChartSeparation
    {
        [DataMember] None,

        [DataMember] CallType,
        [DataMember] Contact,
        [DataMember] TeleCode,
        [DataMember] Line,
        [DataMember] ExtLine,

        [DataMember] CustCol1,
        [DataMember] CustCol2,
        [DataMember] CustCol3,
        [DataMember] CustCol4,
        [DataMember] CustCol5,
        [DataMember] CustCol6,
        [DataMember] CustCol7,
        [DataMember] CustCol8,
        [DataMember] CustCol9,
        [DataMember] CustCol0
    }
}
