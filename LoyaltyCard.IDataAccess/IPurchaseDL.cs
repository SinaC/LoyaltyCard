using System;
using System.Collections.Generic;
using LoyaltyCard.Domain;

namespace LoyaltyCard.IDataAccess
{
    public interface IPurchaseDL
    {
        List<Purchase> GetPurchases(Guid clientId);
        void SavePurchase(Purchase purchase);
        void DeletePurchase(Purchase purchase);

        decimal GetDaySales();
        decimal GetWeekSales();
    }
}
