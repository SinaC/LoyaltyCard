using System.Threading.Tasks;

namespace LoyaltyCard.IBusiness
{
    public interface IMailAutomationBL
    {
        Task SendAutomatedMailsAsync();
    }
}
