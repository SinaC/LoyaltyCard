using System;
using System.Runtime.Serialization;

namespace LoyaltyCard.Domain
{
    [DataContract(Namespace = "")]
    public class ClientSummary
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public int ClientId { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public decimal? TotalSinceLastVoucher { get; set; }

        [DataMember]
        public decimal? Total { get; set; }

        [DataMember]
        public DateTime? LastPurchaseDate { get; set; }

        [DataMember]
        public decimal? LastPurchaseAmount { get; set; }

        [DataMember]
        public bool IsBirthDay { get; set; }
    }
}
