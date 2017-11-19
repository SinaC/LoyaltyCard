using System.Runtime.Serialization;

namespace LoyaltyCard.Domain
{
    [DataContract(Namespace = "")]
    public class FooterData
    {
        [DataMember]
        public int ClientCount { get; set; }

        [DataMember]
        public int NewClientCount { get; set; }
    }
}
