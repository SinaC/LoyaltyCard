using System;
using System.Collections.Generic;
using Loyalty.IBusiness;
using LoyaltyCard.Domain;
using Loyalty.IDataAccess;

namespace LoyaltyCard.Business
{
    public class ClientBL : IClientBL
    {
        private IClientDL ClientDL => EasyIoc.IocContainer.Default.Resolve<IClientDL>();

        public List<Client> GetClients()
        {
            return ClientDL.GetClients();
        }

        public Client GetClient(Guid id)
        {
            return ClientDL.GetClient(id);
        }

        public List<Client> SearchClients(string firstNameFilter, string lastNameFilter)
        {
            return ClientDL.SearchClients(firstNameFilter, lastNameFilter);
        }

        public void SaveClient(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            if (client.Id == Guid.Empty)
                client.Id = Guid.NewGuid();

            client.FirstName = client.FirstName.Trim();
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
                throw new ArgumentException("Cannot add a purchase on an unregistered client", nameof(client));
            if (purchase.ClientId == Guid.Empty)
                purchase.ClientId = client.Id;
            if (purchase.ClientId != client.Id)
                throw new ArgumentException("Cannot save a purchase on a different client", nameof(purchase));

            if (purchase.Id == Guid.Empty)
                purchase.Id = Guid.NewGuid();

            ClientDL.SavePurchase(client, purchase);
        }
    }
}
