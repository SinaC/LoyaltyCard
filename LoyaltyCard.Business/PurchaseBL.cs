using System;
using LoyaltyCard.Domain;
using LoyaltyCard.IBusiness;
using LoyaltyCard.IDataAccess;

namespace LoyaltyCard.Business
{
    public class PurchaseBL : IPurchaseBL
    {
        private IPurchaseDL PurchaseDL => EasyIoc.IocContainer.Default.Resolve<IPurchaseDL>();

        public void SavePurchase(Purchase purchase)
        {
            if (purchase == null)
                throw new ArgumentNullException(nameof(purchase));
            if (purchase.ClientId == Guid.Empty)
                throw new ArgumentException("Purchase clientId cannot be empty", nameof(purchase));

            if (purchase.Id == Guid.Empty)
                purchase.Id = Guid.NewGuid();

            PurchaseDL.SavePurchase(purchase);
        }

        public void DeletePurchase(Purchase purchase)
        {
            if (purchase == null)
                throw new ArgumentNullException(nameof(purchase));
            if (purchase.Id == Guid.Empty)
                throw new ArgumentException("Purchase id cannot be empty", nameof(purchase));
            if (purchase.ClientId == Guid.Empty)
                throw new ArgumentException("Purchase clientId cannot be empty", nameof(purchase));

            PurchaseDL.DeletePurchase(purchase);
        }
    }
}
