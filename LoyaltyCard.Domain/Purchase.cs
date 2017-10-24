using System;
using System.Runtime.Serialization;

namespace LoyaltyCard.Domain
{
    [DataContract]
    public class Purchase
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid ClientId { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public decimal Amount { get; set; }
    }
}
