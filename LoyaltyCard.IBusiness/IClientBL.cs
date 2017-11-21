using System;
using System.Collections.Generic;
using LoyaltyCard.Domain;

namespace LoyaltyCard.IBusiness
{
    public interface IClientBL
    {
        List<ClientSummary> SearchClientSummaries(string filter);

        Client GetClient(Guid id);

        void SaveClient(Client client);

        List<Client> GetClients(Func<Client, bool> filterFunc);

        Client CreateClient();

        void DeleteClient(Client client);
    }
}
