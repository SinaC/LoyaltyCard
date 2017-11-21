using System;
using System.Collections.Generic;
using System.Linq;
using LoyaltyCard.Domain;
using LoyaltyCard.IDataAccess;

namespace LoyaltyCard.DataAccess.FileBased
{
    // Crappy workaround because Client and Purchases are stored in the same file
    public partial class ClientDL : IPurchaseDL
    {
        public List<Purchase> GetPurchases(Guid clientId)
        {
            Client client = GetClient(clientId);

            return (client?.Purchases ?? Enumerable.Empty<Purchase>()).ToList();
        }

        public void SavePurchase(Purchase purchase)
        {
            Client client = GetClient(purchase.ClientId);
            if (client.Purchases.All(p => p.Id != purchase.Id))
                client.Purchases.Add(purchase);
            SaveClient(client);
        }

        public void DeletePurchase(Purchase purchase)
        {
            Client client = GetClient(purchase.ClientId);
            client.Purchases.Remove(purchase);
            SaveClient(client);
        }

        public decimal GetDaySales()
        {
            LoadClients(); // Load clients if needed

            DateTime today = DateTime.Today;

            decimal daySales = _clients.Where(x => x.Purchases?.Any() == true).SelectMany(x => x.Purchases).Where(p => p.Date.Date == today).Sum(p => p.Amount);

            return daySales;
        }

        public decimal GetWeekSales()
        {
            LoadClients(); // Load clients if needed

            DateTime today = DateTime.Today;
            DateTime weekStart = today.AddDays(-(int)DateTime.Today.DayOfWeek);
            DateTime weekEnd = weekStart.AddDays(7).AddSeconds(-1);

            decimal weekSales = _clients.Where(x => x.Purchases?.Any() == true).SelectMany(x => x.Purchases).Where(p => p.Date >= weekStart && p.Date <= weekEnd).Sum(p => p.Amount);

            return weekSales;
        }
    }
}
