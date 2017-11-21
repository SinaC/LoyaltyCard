using System;
using System.Collections.Generic;
using System.Linq;
using LoyaltyCard.Domain;
using LoyaltyCard.IDataAccess;

namespace LoyaltyCard.DataAccess.FileBased
{
    public partial class ClientDL : IVoucherDL
    {
        public List<Voucher> GetVouchers(Guid clientId)
        {
            Client client = GetClient(clientId);
            return (client?.Vouchers ?? Enumerable.Empty<Voucher>()).ToList();
        }

        public void SaveVoucher(Voucher voucher)
        {
            Client client = GetClient(voucher.ClientId);
            if (client.Vouchers.All(p => p.Id != voucher.Id))
                client.Vouchers.Add(voucher);
            SaveClient(client);
        }
    }
}
