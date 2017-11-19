using System;
using System.Collections.Generic;
using System.Globalization;
using LoyaltyCard.Domain;

namespace LoyaltyCard.IBusiness
{
    public interface IClientBL
    {
        List<ClientSummary> GetClientSummaries(string filter);

        List<Client> GetClients();

        Client GetClient(Guid id);

        List<Client> SearchClients(string firstNameFilter, string lastNameFilter);

        List<Client> SearchClients(string filter);

        void SaveClient(Client client);

        void SavePurchase(Client client, Purchase purchase);

        List<Client> GetClients(Func<Client, bool> filterFunc);

        void DeleteClient(Client client);
    }
}
