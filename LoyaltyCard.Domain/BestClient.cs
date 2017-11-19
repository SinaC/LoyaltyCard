using System.Runtime.Serialization;

namespace LoyaltyCard.Domain
{
    [DataContract(Namespace = "")]
    public class BestClient
    {
        [DataMember]
        public Client Client { get; set; }

        [DataMember]
        public decimal? Amount { get; set; }
    }
}
