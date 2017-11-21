using System;
using System.Threading.Tasks;
using LoyaltyCard.Domain;

namespace LoyaltyCard.IMailSender
{
    public interface IMailSender
    {
        Task SendHappyBirthdayMailAsync(string recipientMail, string firstName, Sex sex, DateTime birthDate);
        Task SendNewClientMailAsync(string recipientMail, string firstName, Sex sex);
        Task SendVoucherMailAsync(string recipientMail, string firstName, Sex sex, decimal discount, DateTime maxValidity);
    }
}
