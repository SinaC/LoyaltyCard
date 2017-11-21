using LoyaltyCard.Domain;

namespace LoyaltyCard.IBusiness
{
    public interface IPurchaseBL
    {
        void SavePurchase(Purchase purchase);
        void DeletePurchase(Purchase purchase);
    }
}
