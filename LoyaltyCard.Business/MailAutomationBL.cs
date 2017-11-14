﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
                    Logger.WriteLine(LogLevels.Info, $"Sending welcome mail to {client.FirstName ?? "???"} at {client.Email}");

                    await MailSender.SendNewClientMailAsync(client.Email, client.FirstName);

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

                    Logger.WriteLine(LogLevels.Info, $"Sending happy birthday to {client.FirstName ?? "???"} at {client.Email} birth date {client.BirthDate.Value:dd/MM/yyyy}");

                    await MailSender.SendHappyBirthdayMailAsync(client.Email, client.FirstName, client.BirthDate);

                    client.LastBirthMailDate = DateTime.Now;
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
