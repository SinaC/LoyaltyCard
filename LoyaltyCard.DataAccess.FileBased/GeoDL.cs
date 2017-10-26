using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using LoyaltyCard.IDataAccess;
using LoyaltyCard.Domain;

namespace LoyaltyCard.DataAccess.FileBased
{
    public class GeoDL : IGeoDL
    {
        private Dictionary<string, string> _cityByZip;

        public string GetCityFromZip(string zip)
        {
            Load(); // load if needed

            string city;
            if (!_cityByZip.TryGetValue(zip, out city))
                return null;
            return city;
        }

        private void Load()
        {
            if (_cityByZip != null)
                return;

            string file = ConfigurationManager.AppSettings["CityByZipFile"];
            if (!File.Exists(file))
            {
                CreateInitialDb();
                Save();
                return;
            }

            List<CityByZip> data;
            using (XmlTextReader reader = new XmlTextReader(file))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(List<CityByZip>));
                data = (List<CityByZip>) serializer.ReadObject(reader);
            }
            _cityByZip = data.ToDictionary(x => x.Zip, x => x.City);
        }

        private void Save()
        {
            if (_cityByZip == null)
                return;

            string file = ConfigurationManager.AppSettings["CityByZipFile"];
            List<CityByZip> cities = _cityByZip.Select(x => new CityByZip
            {
                Zip = x.Key,
                City = x.Value
            }).ToList();
            using (XmlTextWriter writer = new XmlTextWriter(file, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                DataContractSerializer serializer = new DataContractSerializer(typeof(List<CityByZip>));
                serializer.WriteObject(writer, cities);
            }
        }

        private void CreateInitialDb()
        {
            _cityByZip = new Dictionary<string, string>
            {
                { "1420", "Braine-L'Alleud"},
                { "1410", "Waterloo"},
            };
        }
    }
}
