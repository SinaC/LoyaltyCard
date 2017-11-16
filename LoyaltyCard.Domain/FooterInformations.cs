using System.Runtime.Serialization;

namespace LoyaltyCard.Domain
{
    [DataContract]
    public class FooterInformations
    {
        [DataMember]
        public int TotalClientCount { get; set; }

        [DataMember]
        public int TotalNewClientCount { get; set; }

        [DataMember]
        public decimal DaySales { get; set; }

        [DataMember]
        public decimal WeekSales { get; set; }
    }
}
