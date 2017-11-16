using System;
using System.Collections.Generic;
using LoyaltyCard.Domain;
using LoyaltyCard.IBusiness;
using LoyaltyCard.IDataAccess;
using LoyaltyCard.Log;

namespace LoyaltyCard.Business
{
    public class StatisticsBL : IStatisticsBL
    {
        private IClientDL ClientDL => EasyIoc.IocContainer.Default.Resolve<IClientDL>();

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
            return ClientDL.GetFooterInformations();
        }
    }
}
