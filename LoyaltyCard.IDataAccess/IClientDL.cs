using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoyaltyCard.Domain;

namespace LoyaltyCard.IDataAccess
{
    public interface IClientDL
    {
        List<Client> GetClients();

        Client GetClient(Guid id);

        List<Client> SearchClients(string firstNameFilter, string lastNameFilter);

        List<Client> SearchClients(string filter);

        void SaveClient(Client client);

        void SavePurchase(Client client, Purchase purchase);

        int GetMaxClientId();

        List<Client> GetClients(Func<Client, bool> filterFunc);

        void DeleteClient(Client client);

        // Statistics

        BestClient GetBestClientInPeriod(DateTime from, DateTime till);

        Dictionary<AgeCategories, int> GetClientCountByAgeCategory();

        Dictionary<Sex, int> GetClientCountBySex();

        Dictionary<AgeCategories, decimal> GetClientAverageAmountByAgeCategory();

        FooterInformations GetFooterInformations();
    }
}
