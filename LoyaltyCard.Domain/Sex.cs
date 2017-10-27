using System.ComponentModel;
using System.Runtime.Serialization;

namespace LoyaltyCard.Domain
{
    [DataContract(Namespace = "")]
    public enum Sex
    {
        [EnumMember]
        [Description("Non-spécifié")]
        Unspecified = 0,
        [EnumMember]
        [Description("Homme")]
        Male = 1,
        [EnumMember]
        [Description("Femme")]
        Female = 2
    }
}
