using System;
using System.Collections.Generic;
using System.Linq;
using LoyaltyCard.Domain;
using LoyaltyCard.IDataAccess;

namespace LoyaltyCard.DataAccess.FileBased
{
    public partial class ClientDL : IVoucherDL
    {
        public List<Voucher> GetClientVouchers(Guid id)
        {
            LoadClients(); // Load clients if needed

            return (_clients.FirstOrDefault(x => x.Id == id)?.Vouchers ?? Enumerable.Empty<Voucher>()).ToList();
        }
    }
}
