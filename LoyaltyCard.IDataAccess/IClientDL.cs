using System;
using System.Collections.Generic;
using LoyaltyCard.Domain;

namespace LoyaltyCard.IDataAccess
{
    public interface IClientDL
    {
        List<ClientSummary> SearchClientSummaries(string filter);

        Client GetClient(Guid id);

        void SaveClient(Client client);

        int GetMaxClientId();

        List<Client> GetClients(Func<Client, bool> filterFunc);

        void DeleteClient(Client client);

        // Statistics
        int GetClientCount();

        int GetNewClientCount();

        BestClient GetBestClientInPeriod(DateTime from, DateTime till);

        Dictionary<AgeCategories, int> GetClientCountByAgeCategory();

        Dictionary<Sex, int> GetClientCountBySex();

        Dictionary<AgeCategories, decimal> GetClientAverageAmountByAgeCategory();
    }
}
