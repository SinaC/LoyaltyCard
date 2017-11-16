using System.ComponentModel;

namespace LoyaltyCard.Domain
{
    public enum AgeCategories
    {
        [Description("Non-spécifié")]
        Undefined,
        [Description("10-")]
        LessThan10,
        [Description("11->15")]
        Between11And15,
        [Description("16->20")]
        Between16And20,
        [Description("21->30")]
        Between20And30,
        [Description("31->40")]
        Between31And40,
        [Description("41->50")]
        Between41And50,
        [Description("51->60")]
        Between51And60,
        [Description("61->70")]
        Between61And70,
        [Description("70+")]
        MoreThan71,
    }
}
