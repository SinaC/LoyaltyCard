using System.Runtime.Serialization;

namespace LoyaltyCard.Domain
{
    [DataContract(Namespace = "")]
    public class CityByZip
    {
        [DataMember]
        public string Zip { get; set; }

        [DataMember]
        public string City { get; set; }
    }
}
