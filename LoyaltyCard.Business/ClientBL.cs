using System;
using System.Collections.Generic;
using LoyaltyCard.IBusiness;
using LoyaltyCard.Domain;
using LoyaltyCard.IDataAccess;
using LoyaltyCard.Log;

namespace LoyaltyCard.Business
{
    public class ClientBL : IClientBL
    {
        private const int FirstClientId = 100000;

        private IClientDL ClientDL => EasyIoc.IocContainer.Default.Resolve<IClientDL>();
        private ILog Logger => EasyIoc.IocContainer.Default.Resolve<ILog>();

        public List<ClientSummary> SearchClientSummaries(string filter)
        {
            return ClientDL.SearchClientSummaries(filter);
        }

        public Client GetClient(Guid id)
        {
            Client client = ClientDL.GetClient(id);

            if (client != null)
                AddMandatoryFields(client);

            return client;
        }

        public void SaveClient(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            AddMandatoryFields(client);

            if (!string.IsNullOrWhiteSpace(client.FirstName))
                client.FirstName = client.FirstName.Trim();
            if (!string.IsNullOrWhiteSpace(client.LastName))
                client.LastName = client.LastName.Trim();

            ClientDL.SaveClient(client);
        }

        public List<Client> GetClients(Func<Client, bool> filterFunc)
        {
            List<Client> clients = ClientDL.GetClients(filterFunc);

            if (clients != null)
            {
                foreach (Client client in clients)
                    AddMandatoryFields(client);
            }

            return clients;
        }

        public Client CreateClient()
        {
            Client client = new Client();
            AddMandatoryFields(client);
            return client;
        }

        public void DeleteClient(Client client)
        {
            ClientDL.DeleteClient(client);
        }

        public void DeleteClient(Guid id)
        {
            ClientDL.DeleteClient(id);
        }

        //
        private void AddMandatoryFields(Client client)
        {
            if (client.Id == Guid.Empty)
                client.Id = Guid.NewGuid();
            if (!client.CreationDate.HasValue)
                client.CreationDate = DateTime.Now;

            if (client.ClientBusinessId == 0)
            {
                int clientBusinessId = ClientDL.GetMaxClientId();
                if (clientBusinessId <= 0)
                    clientBusinessId = FirstClientId;
                else
                    clientBusinessId = clientBusinessId + 1;
                client.ClientBusinessId = clientBusinessId;
            }
        }
    }
}
