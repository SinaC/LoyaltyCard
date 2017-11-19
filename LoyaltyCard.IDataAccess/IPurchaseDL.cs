using System;
using System.Collections.Generic;
using LoyaltyCard.Domain;

namespace LoyaltyCard.IDataAccess
{
    public interface IPurchaseDL
    {
        List<Purchase> GetClientPurchases(Guid id);
        decimal GetDaySales();
        decimal GetWeekSales();
    }
}
