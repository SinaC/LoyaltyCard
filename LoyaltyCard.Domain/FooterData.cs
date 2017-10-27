using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCard.Domain
{
    [DataContract]
    public class FooterData
    {
        [DataMember]
        public int ClientCount { get; set; }

        [DataMember]
        public int NewClientCount { get; set; }
    }
}
