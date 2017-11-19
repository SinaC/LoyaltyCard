using System;
using System.Collections.Generic;
using LoyaltyCard.Domain;
using LoyaltyCard.IBusiness;
using LoyaltyCard.IDataAccess;

namespace LoyaltyCard.Business
{
    public class StatisticsBL : IStatisticsBL
    {
        private IClientDL ClientDL => EasyIoc.IocContainer.Default.Resolve<IClientDL>();
        private IPurchaseDL PurchaseDL => EasyIoc.IocContainer.Default.Resolve<IPurchaseDL>();

        public BestClient GetBestClientInPeriod(DateTime from, DateTime till)
        {
            return ClientDL.GetBestClientInPeriod(from, till);
        }

        public Dictionary<AgeCategories, int> GetClientCountByAgeCategory()
        {
            return ClientDL.GetClientCountByAgeCategory();
        }

        public Dictionary<Sex, int> GetClientCountBySex()
        {
            return ClientDL.GetClientCountBySex();
        }

        public Dictionary<AgeCategories, decimal> GetClientAverageAmountByAgeCategory()
        {
            return ClientDL.GetClientAverageAmountByAgeCategory();
        }

        public FooterInformations GetFooterInformations()
        {
            int clientCount = ClientDL.GetClientCount();
            int newClientCount = ClientDL.GetNewClientCount();
            decimal daySales = PurchaseDL.GetDaySales();
            decimal weekSales = PurchaseDL.GetWeekSales();

            return new FooterInformations
            {
                TotalClientCount = clientCount,
                TotalNewClientCount = newClientCount,
                DaySales = daySales,
                WeekSales = weekSales
            };
        }
    }
}
