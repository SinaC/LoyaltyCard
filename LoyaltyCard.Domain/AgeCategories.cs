using System.ComponentModel;
using System.Runtime.Serialization;

namespace LoyaltyCard.Domain
{
    [DataContract(Namespace = "")]
    public enum AgeCategories
    {
        [Description("Non-spécifié")]
        [EnumMember]
        Undefined,
        [Description("10-")]
        [EnumMember]
        LessThan10,
        [Description("11->15")]
        [EnumMember]
        Between11And15,
        [Description("16->20")]
        [EnumMember]
        Between16And20,
        [Description("21->30")]
        [EnumMember]
        Between20And30,
        [Description("31->40")]
        [EnumMember]
        Between31And40,
        [Description("41->50")]
        [EnumMember]
        Between41And50,
        [Description("51->60")]
        [EnumMember]
        Between51And60,
        [Description("61->70")]
        [EnumMember]
        Between61And70,
        [Description("70+")]
        [EnumMember]
        MoreThan71,
    }
}
