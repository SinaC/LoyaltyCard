using System;
using System.Collections.Generic;
using LoyaltyCard.IBusiness;
using LoyaltyCard.Domain;
using LoyaltyCard.IDataAccess;

namespace LoyaltyCard.Business
{
    public class ClientBL : IClientBL
    {
        private const int FirstClientId = 100000;

        private IClientDL ClientDL => EasyIoc.IocContainer.Default.Resolve<IClientDL>();

        public List<ClientSummary> GetClientSummaries(string filter)
        {
            return ClientDL.GetClientSummaries(filter);
        }

        public List<Client> GetClients()
        {
            List<Client> clients = ClientDL.GetClients();

            if (clients != null)
            {
                foreach (Client client in clients)
                    AddMandatoryFields(client);
            }

            return clients;
        }

        public Client GetClient(Guid id)
        {
            Client client = ClientDL.GetClient(id);

            if (client != null)
                AddMandatoryFields(client);

            return client;
        }

        public List<Client> SearchClients(string firstNameFilter, string lastNameFilter)
        {
            List<Client> clients = ClientDL.SearchClients(firstNameFilter, lastNameFilter);

            if (clients != null)
            {
                foreach (Client client in clients)
                    AddMandatoryFields(client);
            }

            return clients;
        }

        public List<Client> SearchClients(string filter)
        {
            List<Client> clients = ClientDL.SearchClients(filter);

            if (clients != null)
            {
                foreach (Client client in clients)
                    AddMandatoryFields(client);
            }

            return clients;
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

        public void SavePurchase(Client client, Purchase purchase)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (purchase == null)
                throw new ArgumentNullException(nameof(purchase));
            if (client.Id == Guid.Empty)
                throw new ArgumentException("Cannot add a purchase to an unregistered client", nameof(client));
            if (purchase.ClientId == Guid.Empty)
                purchase.ClientId = client.Id;
            if (purchase.ClientId != client.Id)
                throw new ArgumentException("Cannot save a purchase to a different client", nameof(purchase));

            if (purchase.Id == Guid.Empty)
                purchase.Id = Guid.NewGuid();

            ClientDL.SavePurchase(client, purchase);
        }

        public void SaveVoucher(Client client, Voucher voucher)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (voucher == null)
                throw new ArgumentNullException(nameof(voucher));
            if (client.Id == Guid.Empty)
                throw new ArgumentException("Cannot add a voucher to an unregistered client", nameof(client));
            if (voucher.ClientId == Guid.Empty)
                voucher.ClientId = client.Id;
            if (voucher.ClientId != client.Id)
                throw new ArgumentException("Cannot save a voucher to a different client", nameof(voucher));

            if (voucher.Id == Guid.Empty)
                voucher.Id = Guid.NewGuid();

            ClientDL.SaveVoucher(client, voucher);
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
