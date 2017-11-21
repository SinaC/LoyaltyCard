using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LoyaltyCard.Common.Extensions;
using LoyaltyCard.Domain;
using LoyaltyCard.IBusiness;
using LoyaltyCard.Log;

namespace LoyaltyCard.Business
{
    public class MailAutomationBL : IMailAutomationBL
    {
        private IClientBL ClientBL => EasyIoc.IocContainer.Default.Resolve<IClientBL>();
        private IMailSender.IMailSender MailSender => EasyIoc.IocContainer.Default.Resolve<IMailSender.IMailSender>();
        private ILog Logger => EasyIoc.IocContainer.Default.Resolve<ILog>();

        public async Task SendAutomatedMailsAsync()
        {
            // Welcome mails
            foreach (Client client in ClientBL.GetClients(c => !string.IsNullOrWhiteSpace(c.Email) && !c.WelcomeMailDate.HasValue && (c.CreationDate ?? DateTime.Today).AddMonths(1) <= DateTime.Today))
            {
                try
                {
                    Logger.Info($"Sending welcome mail to {client.FirstName ?? "???"} at {client.Email}");

                    await MailSender.SendNewClientMailAsync(client.Email, client.FirstName, client.Sex);

                    client.WelcomeMailDate = DateTime.Now;
                    ClientBL.SaveClient(client);
                }
                catch (Exception ex)
                {
                    Logger.Exception($"Failed to send welcome mail to {client.Email}", ex);
                }
            }

            // Birthday mails
            foreach (Client client in ClientBL.GetClients(c => !string.IsNullOrWhiteSpace(c.Email) && c.BirthDate.HasValue && IsBirthDayInThePast(c)))
            {
                try
                {
                    Debug.Assert(client.BirthDate.HasValue);

                    Logger.Info($"Sending happy birthday to {client.FirstName ?? "???"} at {client.Email} birth date {client.BirthDate.Value:dd/MM/yyyy}");

                    await MailSender.SendHappyBirthdayMailAsync(client.Email, client.FirstName, client.Sex, client.BirthDate.Value);

                    client.LastBirthMailDate = DateTime.Today.AddDays(1);
                    ClientBL.SaveClient(client);
                }
                catch (Exception ex)
                {
                    Logger.Exception($"Failed to send happy birthday mail to {client.Email}", ex);
                }
            }
        }

        private bool IsBirthDayInThePast(Client client)
        {
            if (!client.BirthDate.HasValue)
                return false;
            DateTime lastBirthDay = client.LastBirthMailDate ?? DateTime.Today.AddDays(-3); // Max 3 days in the past
            return client.BirthDate.Value.IsBirthdayInRange(lastBirthDay, DateTime.Today.AddDays(1).AddMilliseconds(-1));
        }

    }
}
