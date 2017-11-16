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

namespace LoyaltyCard.DataAccess.FileBased
{
    public class ClientDL : IClientDL
    {
        private List<Client> _clients;

        private Func<Client, DateTime, DateTime, decimal?> PurchaseInPeriodFunc => (client, from, till) => client.Purchases?.Where(p => p.Date >= from && p.Date <= till).SumNullIfEmpty(p => p.Amount);

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

            IQueryable<Client> query = _clients.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                string[] tokens = filter.Split(' ');

                foreach (string token in tokens)
                    query = query.Where(c => Contains(c.FirstName ?? string.Empty, token) ||
                    Contains(c.LastName ?? string.Empty, token) ||
                    Contains(c.Email ?? string.Empty, token) ||
                    c.ClientId.ToString().StartsWith(token));
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
            SaveClient(client);
        }

        public int GetMaxClientId()
        {
            LoadClients(); // Load clients if needed

            //return _clients.Max(x => x.ClientId);
            return _clients.Count == 0 ? -1 : _clients.Max(x => x.ClientId);
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

            var averageAmountByAgeCategories = _clients.Select(client =>
                    new
                    {
                        client,
                        category = client.AgeCategory
                    })
                .GroupBy(x => x.category)
                .ToDictionary(g => g.Key, g => g.Where(x => x.client?.Purchases.Any() == true).Average(x => x.client.Purchases?.Average(p => p.Amount)) ?? 0);

            return averageAmountByAgeCategories;
        }

        public FooterInformations GetFooterInformations()
        {
            LoadClients(); // Load clients if needed

            DateTime today = DateTime.Today;
            DateTime weekStart = today.AddDays(-(int)DateTime.Today.DayOfWeek);
            DateTime weekEnd = weekStart.AddDays(7).AddSeconds(-1);

            int clientCount = _clients.Count;
            int newClientCount = _clients.Count(x => x.CreationDate.HasValue && x.CreationDate.Value.Date == today);
            decimal daySales = _clients.Where(x => x.Purchases?.Any() == true).SelectMany(x => x.Purchases).Where(p => p.Date.Date == today).Sum(p => p.Amount);
            decimal weekSales = _clients.Where(x => x.Purchases?.Any() == true).SelectMany(x => x.Purchases).Where(p => p.Date >= weekStart && p.Date <= weekEnd).Sum(p => p.Amount);

            return new FooterInformations
            {
                TotalClientCount = clientCount,
                TotalNewClientCount = newClientCount,
                DaySales = daySales,
                WeekSales = weekSales
            };
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

        private bool Contains(string s, string filter)
        {
            //http://stackoverflow.com/questions/359827/ignoring-accented-letters-in-string-comparison/7720903#7720903
            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(s, filter, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) >= 0;
        }
    }
}
