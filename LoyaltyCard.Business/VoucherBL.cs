using System;
using LoyaltyCard.Domain;
using LoyaltyCard.IBusiness;
using LoyaltyCard.IDataAccess;

namespace LoyaltyCard.Business
{
    public class VoucherBL : IVoucherBL
    {
        private IVoucherDL VoucherDL => EasyIoc.IocContainer.Default.Resolve<IVoucherDL>();

        public void SaveVoucher(Voucher voucher)
        {
            if (voucher == null)
                throw new ArgumentNullException(nameof(voucher));
            if (voucher.ClientId == Guid.Empty)
                throw new ArgumentException("Cannot add a voucher to an empty client", nameof(voucher));

            if (voucher.Id == Guid.Empty)
                voucher.Id = Guid.NewGuid();

            VoucherDL.SaveVoucher(voucher);
        }
    }
}
