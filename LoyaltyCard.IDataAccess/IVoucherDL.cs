using System;
using System.Collections.Generic;
using LoyaltyCard.Domain;

namespace LoyaltyCard.IDataAccess
{
    public interface IVoucherDL
    {
        List<Voucher> GetVouchers(Guid clientId);
        void SaveVoucher(Voucher voucher);
    }
}
