using System.ComponentModel;

namespace LoyaltyCard.Domain
{
    public enum ClientCategories
    {
        [Description("Vallée-Bailly")]
        Ivb = 0,
        [Description("Eneo")]
        Eneo = 1,
        [Description("Ecole des arts")]
        EcoleDesArts = 2,
        [Description("Grange des champs")]
        GrangeDesChamps = 3,
        [Description("Atelier Lillois")]
        AtelierLillois = 4,
        [Description("Atelier Gibloux")]
        AtelierGibloux = 5
    }
}
