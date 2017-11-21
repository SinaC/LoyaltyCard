using System;
using System.Collections.Generic;
using LoyaltyCard.Domain;

namespace LoyaltyCard.IDataAccess
{
    public interface IClientDL
    {
        List<ClientSummary> GetClientSummaries(string filter);

        List<Client> GetClients();

        Client GetClient(Guid id);

        List<Client> SearchClients(string firstNameFilter, string lastNameFilter);

        List<Client> SearchClients(string filter);

        void SaveClient(Client client);

        void SavePurchase(Client client, Purchase purchase);

        void SaveVoucher(Client client, Voucher voucher);

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
