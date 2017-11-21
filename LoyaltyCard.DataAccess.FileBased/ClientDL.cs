using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using LoyaltyCard.Common.Extensions;
using LoyaltyCard.Domain;
using LoyaltyCard.IDataAccess;
using LoyaltyCard.Log;

namespace LoyaltyCard.DataAccess.FileBased
{
    public partial class ClientDL : IClientDL
    {
        private List<Client> _clients;

        private Func<Client, DateTime, DateTime, decimal?> PurchaseInPeriodFunc => (client, from, till) => client.Purchases?.Where(p => p.Date >= from && p.Date <= till).SumNullIfEmpty(p => p.Amount);

        private ILog Logger => EasyIoc.IocContainer.Default.Resolve<ILog>();

        public List<ClientSummary> GetClientSummaries(string filter)
        {
            LoadClients(); // load clients if needed

            IEnumerable<Client> query = _clients.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                string[] tokens = filter.Split(' ');

                foreach (string token in tokens)
                    query = query.Where(c => Contains(c.FirstName ?? string.Empty, token) ||
                                             Contains(c.LastName ?? string.Empty, token) ||
                                             Contains(c.Email ?? string.Empty, token) ||
                                             c.ClientBusinessId.ToString().StartsWith(token));
            }

            return query.Select(x => new ClientSummary(x)).ToList();
        }

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

            IEnumerable<Client> query = _clients.AsQueryable();
            if (!string.IsNullOrWhiteSpace(firstNameFilter))
            {
                firstNameFilter = firstNameFilter.ToLowerInvariant();
                query = query.Where(x => !string.IsNullOrWhiteSpace(x.FirstName) && x.FirstName.ToLowerInvariant().StartsWith(firstNameFilter));
            }
            if (!string.IsNullOrWhiteSpace(lastNameFilter))
            {
                lastNameFilter = lastNameFilter.ToLowerInvariant();
                query = query.Where(x => !string.IsNullOrWhiteSpace(x.LastName) && x.LastName.ToLowerInvariant().StartsWith(lastNameFilter));
            }

            return query.ToList();
        }

        public List<Client> SearchClients(string filter)
        {
            LoadClients(); // Load clients if needed

            IEnumerable<Client> query = _clients.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                string[] tokens = filter.Split(' ');

                foreach (string token in tokens)
                    query = query.Where(c => Contains(c.FirstName ?? string.Empty, token) ||
                    Contains(c.LastName ?? string.Empty, token) ||
                    Contains(c.Email ?? string.Empty, token) ||
                    c.ClientBusinessId.ToString().StartsWith(token));
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
                // Merge vouchers
                existingClient.Vouchers = existingClient.Vouchers ?? new ObservableCollection<Voucher>();
                if (client.Vouchers?.Any() == true)
                {
                    foreach (Voucher voucher in client.Vouchers)
                        if (existingClient.Vouchers.All(p => p.Id != voucher.Id))
                            existingClient.Vouchers.Add(voucher);
                }
            }

            SaveClients();
        }

        public void SavePurchase(Client client, Purchase purchase)
        {
            SaveClient(client);
        }

        public void SaveVoucher(Client client, Voucher voucher)
        {
            SaveClient(client);
        }

        public int GetMaxClientId()
        {
            LoadClients(); // Load clients if needed

            //return _clients.Max(x => x.ClientId);
            return _clients.Count == 0 ? -1 : _clients.Max(x => x.ClientBusinessId);
        }

        public List<Client> GetClients(Func<Client, bool> filterFunc)
        {
            LoadClients(); // Load clients if needed

            return _clients.Where(filterFunc).ToList();
        }

        public void DeleteClient(Client client)
        {
            LoadClients(); // Load clients if needed

            _clients.RemoveAll(x => x.Id == client.Id);

            SaveClients();
        }

        // Statistics

        public int GetClientCount()
        {
            LoadClients(); // Load clients if needed

            int clientCount = _clients.Count;

            return clientCount;
        }

        public int GetNewClientCount()
        {
            LoadClients(); // Load clients if needed

            DateTime today = DateTime.Today;

            int newClientCount = _clients.Count(x => x.CreationDate.HasValue && x.CreationDate.Value.Date == today);

            return newClientCount;
        }

        public BestClient GetBestClientInPeriod(DateTime from, DateTime till)
        {
            LoadClients(); // Load clients if needed

            var weekPurchaseByClient = _clients.Select(client =>
                    new
                    {
                        client,
                        total = PurchaseInPeriodFunc(client, from, till)
                    })
                .Where(x => x.total.HasValue)
                .WhereMax(x => x.total.Value);
            BestClient bestClient = null;
            if (weekPurchaseByClient != null)
            {
                bestClient = new BestClient
                {
                    Client = weekPurchaseByClient.client,
                    Amount = weekPurchaseByClient.total
                };
            }
            return bestClient;
        }

        public Dictionary<AgeCategories, int> GetClientCountByAgeCategory()
        {
            LoadClients(); // Load clients if needed

            var clientCountByAgeCategories = _clients.Select(client =>
                    new
                    {
                        client,
                        category = client.AgeCategory
                    })
                .GroupBy(x => x.category)
                .ToDictionary(g => g.Key, g => g.Count());

            return clientCountByAgeCategories;
        }

        public Dictionary<Sex, int> GetClientCountBySex()
        {
            LoadClients(); // Load clients if needed

            var clientCountBySex = _clients.Select(client =>
                    new
                    {
                        client,
                        sex = client.Sex
                    })
                .GroupBy(x => x.sex)
                .ToDictionary(g => g.Key, g => g.Count());

            return clientCountBySex;
        }

        public Dictionary<AgeCategories, decimal> GetClientAverageAmountByAgeCategory()
        {
            LoadClients(); // Load clients if needed

            var averageAmountByAgeCategories = _clients.Where(x => x.Purchases?.Any() == true).Select(client =>
                    new
                    {
                        averageAmount = client.Purchases.Average(p => p.Amount),
                        category = client.AgeCategory
                    })
                .GroupBy(x => x.category)
                .ToDictionary(g => g.Key, g => g.Average(x => x.averageAmount));

            return averageAmountByAgeCategories;
        }

        //
        public void Backup()
        {
            try
            {
                Logger.Info("Performing client DB backup");

                string clientFile = ConfigurationManager.AppSettings["ClientFile"];
                string backupFile = Path.Combine(Path.GetDirectoryName(clientFile)+$@"\backup\client_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid()}.xml");

                File.Copy(clientFile, backupFile);

                Logger.Info("Client DB backup done");
            }
            catch (Exception ex)
            {
                Logger.Exception("DB backup failed", ex);
            }
        }

        //

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

        private bool Contains(string s, string filter)
        {
            //http://stackoverflow.com/questions/359827/ignoring-accented-letters-in-string-comparison/7720903#7720903
            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(s, filter, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) >= 0;
        }
    }
}
