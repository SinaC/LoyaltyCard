using System;
using System.Collections.Generic;
using LoyaltyCard.Domain;

namespace LoyaltyCard.IDataAccess
{
    public interface IClientDL
    {
        List<Client> GetClients();

        Client GetClient(Guid id);

        List<Client> SearchClients(string firstNameFilter, string lastNameFilter);

        void SaveClient(Client client);

        void SavePurchase(Client client, Purchase purchase);

        int GetMaxClientId();

        List<Client> GetClients(Func<Client, bool> filterFunc);
    }
}
