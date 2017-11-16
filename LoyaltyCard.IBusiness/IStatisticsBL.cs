using System;
using System.Collections.Generic;
using LoyaltyCard.Domain;

namespace LoyaltyCard.IBusiness
{
    public interface IStatisticsBL
    {
        BestClient GetBestClientInPeriod(DateTime from, DateTime till);

        Dictionary<AgeCategories, int> GetClientCountByAgeCategory();

        Dictionary<Sex, int> GetClientCountBySex();

        Dictionary<AgeCategories, decimal> GetClientAverageAmountByAgeCategory();

        FooterInformations GetFooterInformations();
    }
}
