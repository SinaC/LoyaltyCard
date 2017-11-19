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
        public List<Purchase> GetClientPurchases(Guid id)
        {
            LoadClients(); // Load clients if needed

            return (_clients.FirstOrDefault(x => x.Id == id)?.Purchases ?? Enumerable.Empty<Purchase>()).ToList();
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
