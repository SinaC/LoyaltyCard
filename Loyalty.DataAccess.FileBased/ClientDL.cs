using LoyaltyCard.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Loyalty.IDataAccess;

namespace Loyalty.DataAccess.FileBased
{
    public class ClientDL : IClientDL
    {
        private List<Client> _clients;

        public List<Client> GetClients()
        {
            LoadClients(); // Load clients if needed

            return _clients;
        }

        public Client GetClient(Guid id)
        {
            LoadClients(); // Load clients if needed

            return _clients.FirstOrDefault(x => x.Id == id);
        }

        public List<Client> SearchClients(string firstNameFilter, string lastNameFilter)
        {
            LoadClients(); // Load clients if needed

            IQueryable<Client> query = _clients.AsQueryable();
            if (!string.IsNullOrWhiteSpace(firstNameFilter))
            {
                firstNameFilter = firstNameFilter.ToLowerInvariant();
                query = query.Where(x => !string.IsNullOrWhiteSpace(x.FirstName) && x.FirstName.ToLowerInvariant().Contains(firstNameFilter));
            }
            if (!string.IsNullOrWhiteSpace(lastNameFilter))
            {
                lastNameFilter = lastNameFilter.ToLowerInvariant();
                query = query.Where(x => !string.IsNullOrWhiteSpace(x.LastName) && x.LastName.ToLowerInvariant().Contains(lastNameFilter));
            }

            return query.ToList();
        }

        public void SaveClient(Client client)
        {
            LoadClients(); // Load clients if needed

            Client existingClient = GetClient(client.Id);

            if (existingClient == null) // if client doesnt exist, add it
                _clients.Add(client);
            else // if client exists, copy information
            {
                existingClient.FirstName = client.FirstName;
                existingClient.LastName = client.LastName;
                existingClient.BirthDate = client.BirthDate;
                existingClient.Email = client.Email;
                existingClient.Mobile = client.Mobile;
                // Merge purchases
                existingClient.Purchases = existingClient.Purchases ?? new ObservableCollection<Purchase>();
                if (client.Purchases?.Any() == true)
                {
                    foreach (Purchase purchase in client.Purchases)
                        if (existingClient.Purchases.All(p => p.Id != purchase.Id))
                            existingClient.Purchases.Add(purchase);
                }
            }

            SaveClients();
        }

        public void SavePurchase(Client client, Purchase purchase)
        {
            client.Purchases = client.Purchases ?? new ObservableCollection<Purchase>();
            client.Purchases.Add(purchase);
            SaveClient(client);
        }

        private void LoadClients()
        {
            if (_clients != null)
                return;

            string clientFile = ConfigurationManager.AppSettings["ClientFile"];
            if (File.Exists(clientFile))
            {
                List<Client> clients;
                using (XmlTextReader reader = new XmlTextReader(clientFile))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(List<Client>));
                    clients = (List<Client>)serializer.ReadObject(reader);
                }
                _clients = clients;
            }
            else
                _clients = new List<Client>();
        }

        private void SaveClients()
        {
            if (_clients == null)
                return;

            string clientFile = ConfigurationManager.AppSettings["ClientFile"];
            using (XmlTextWriter writer = new XmlTextWriter(clientFile, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                DataContractSerializer serializer = new DataContractSerializer(typeof(List<Client>));
                serializer.WriteObject(writer, _clients);
            }
        }
    }
}
