using System;
using System.Threading.Tasks;
using LoyaltyCard.Domain;
using LoyaltyCard.IBusiness;

namespace LoyaltyCard.Business
{
    public class MailAutomationBL : IMailAutomationBL
    {
        private IClientBL ClientBL => EasyIoc.IocContainer.Default.Resolve<IClientBL>();
        private IMailSender.IMailSender MailSender => EasyIoc.IocContainer.Default.Resolve<IMailSender.IMailSender>();

        public async Task SendAutomatedMailsAsync()
        {
            // Welcome mails
            foreach (Client client in ClientBL.GetClients(c => !string.IsNullOrWhiteSpace(c.Email) && !c.WelcomeMailDate.HasValue && (c.CreationDate ?? DateTime.Today).AddMonths(1) >= DateTime.Today))
            {
                try
                {
                    await MailSender.SendNewClientMailAsync(client.Email, client.FirstName);

                    client.WelcomeMailDate = DateTime.Now;
                    ClientBL.SaveClient(client);
                }
                catch (Exception ex)
                {
                    // TODO: log
                }
            }

            // Birthday mails
            foreach (Client client in ClientBL.GetClients(c => !string.IsNullOrWhiteSpace(c.Email) && IsBirthDayInThePast(c)))
            {
                try
                {
                    await MailSender.SendHappyBirthdayMailAsync(client.Email, client.FirstName, client.BirthDate);

                    client.LastBirthMailDate = DateTime.Now;
                    ClientBL.SaveClient(client);
                }
                catch (Exception ex)
                {
                    // TODO: log
                }
            }
        }

        private bool IsBirthDayInThePast(Client client)
        {
            return false;

            //// TODO
            //if (!client.BirthDate.HasValue)
            //    return false;
            //DateTime now = DateTime.Now;
            //DateTime birth = client.BirthDate.Value;
            //DateTime lastWish = client.LastBirthMailSent ?? DateTime.Now;
            //if (
            //    ((now.Month == birth.Month && now.Day >= birth.Day) || now.Month > birth.Month) && // after birth day

            //    now > lastWish // not already wished
            //    )
            //    return true;
            //return false;
        }
    }
}
