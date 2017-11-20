using LoyaltyCard.Domain;

namespace LoyaltyCard.App.Messages
{
    public class SwitchToDisplayClientMessage
    {
        public ClientSummary Client { get; set; }
    }
}
