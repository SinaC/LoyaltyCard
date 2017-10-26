using System.ComponentModel;
using System.Runtime.Serialization;

namespace LoyaltyCard.Domain
{
    [DataContract(Namespace = "")]
    public enum ClientCategories
    {
        [Description("Vallée-Bailly")]
        [EnumMember]
        Ivb = 0,

        [Description("Eneo")]
        [EnumMember]
        Eneo = 1,

        [Description("Ecole des arts")]
        [EnumMember]
        EcoleDesArts = 2,

        [Description("Grange des champs")]
        [EnumMember]
        GrangeDesChamps = 3,

        [Description("Atelier Lillois")]
        [EnumMember]
        AtelierLillois = 4,

        [Description("Atelier Gibloux")]
        [EnumMember]
        AtelierGibloux = 5
    }
}
