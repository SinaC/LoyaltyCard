using System;
using System.Threading.Tasks;

namespace LoyaltyCard.IMailSender
{
    public interface IMailSender
    {
        Task SendHappyBirthdayMailAsync(string recipientMail, string firstName, DateTime? birthDate);
        Task SendNewClientMailAsync(string recipientMail, string firstName);
        Task SendVoucherMailAsync(string recipientMail, string firstName, decimal discount);
    }
}
