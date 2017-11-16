﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyCard.Business;
using LoyaltyCard.Domain;
using LoyaltyCard.IBusiness;
using LoyaltyCard.IDataAccess;
using LoyaltyCard.Log;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoyaltyCard.Tests
{
    [TestClass]
    public class BirthDayTests
    {
        [TestInitialize]
        public void Initialize()
        {
            EasyIoc.IocContainer.Default.Reset();
        }

        [TestMethod]
        public void BirthDayNotInDateRange()
        {
            MailAutomationBL bl = new MailAutomationBL();
            DateTime birthDay = new DateTime(1976, 12, 07);
            DateTime from = new DateTime(2017,01,01);
            DateTime to = new DateTime(2017,11,15);

            bool isInRange = bl.IsBirthdayInRange(birthDay, from, to);

            Assert.IsFalse(isInRange);
        }

        [TestMethod]
        public void BirthDayInDateRange()
        {
            MailAutomationBL bl = new MailAutomationBL();
            DateTime birthDay = new DateTime(1976, 11, 07);
            DateTime from = new DateTime(2017, 01, 01);
            DateTime to = new DateTime(2017, 11, 15);

            bool isInRange = bl.IsBirthdayInRange(birthDay, from, to);

            Assert.IsTrue(isInRange);
        }

        [TestMethod]
        public void TestClientBirthDayIsToday()
        {
            ClientDLMock clientDLMock = new ClientDLMock();
            MailSenderMock mailSenderMock = new MailSenderMock();
            LogMock logMock = new LogMock();
            EasyIoc.IocContainer.Default.RegisterInstance<IClientBL>(new ClientBL());
            EasyIoc.IocContainer.Default.RegisterInstance<IClientDL>(clientDLMock);
            EasyIoc.IocContainer.Default.RegisterInstance<IMailSender.IMailSender>(mailSenderMock);
            EasyIoc.IocContainer.Default.RegisterInstance<ILog>(logMock);
            MailAutomationBL bl = new MailAutomationBL();
            clientDLMock.AddClient(new Client
            {
                Id = Guid.NewGuid(),
                ClientId = 1,
                FirstName = "Test",
                BirthDate = DateTime.Today.AddYears(-10),
                Email = "pouet@brol.net"
            });

            bl.SendAutomatedMailsAsync().Wait();

            Assert.IsTrue(mailSenderMock.Mails.Count == 1);
            Assert.IsTrue(mailSenderMock.Mails.Single().Contains("BIRTHDAY_"));
        }

        [TestMethod]
        public void TestClientBirthDayIsInTheFuture()
        {
            ClientDLMock clientDLMock = new ClientDLMock();
            MailSenderMock mailSenderMock = new MailSenderMock();
            LogMock logMock = new LogMock();
            EasyIoc.IocContainer.Default.RegisterInstance<IClientBL>(new ClientBL());
            EasyIoc.IocContainer.Default.RegisterInstance<IClientDL>(clientDLMock);
            EasyIoc.IocContainer.Default.RegisterInstance<IMailSender.IMailSender>(mailSenderMock);
            EasyIoc.IocContainer.Default.RegisterInstance<ILog>(logMock);
            MailAutomationBL bl = new MailAutomationBL();
            clientDLMock.AddClient(new Client
            {
                Id = Guid.NewGuid(),
                ClientId = 1,
                FirstName = "Test",
                BirthDate = DateTime.Today.AddYears(-10).AddDays(5),
                Email = "pouet@brol.net"
            });

            bl.SendAutomatedMailsAsync().Wait();

            Assert.IsTrue(mailSenderMock.Mails.Count == 0);
        }

        [TestMethod]
        public void TestClientBirthDayIsInThePastWithin3Days()
        {
            ClientDLMock clientDLMock = new ClientDLMock();
            MailSenderMock mailSenderMock = new MailSenderMock();
            LogMock logMock = new LogMock();
            EasyIoc.IocContainer.Default.RegisterInstance<IClientBL>(new ClientBL());
            EasyIoc.IocContainer.Default.RegisterInstance<IClientDL>(clientDLMock);
            EasyIoc.IocContainer.Default.RegisterInstance<IMailSender.IMailSender>(mailSenderMock);
            EasyIoc.IocContainer.Default.RegisterInstance<ILog>(logMock);
            MailAutomationBL bl = new MailAutomationBL();
            clientDLMock.AddClient(new Client
            {
                Id = Guid.NewGuid(),
                ClientId = 1,
                FirstName = "Test",
                BirthDate = DateTime.Today.AddYears(-10).AddDays(-2),
                Email = "pouet@brol.net"
            });

            bl.SendAutomatedMailsAsync().Wait();

            Assert.IsTrue(mailSenderMock.Mails.Count == 1);
            Assert.IsTrue(mailSenderMock.Mails.Single().Contains("BIRTHDAY_"));
        }

        [TestMethod]
        public void TestClientBirthDayIsInThePast()
        {
            ClientDLMock clientDLMock = new ClientDLMock();
            MailSenderMock mailSenderMock = new MailSenderMock();
            LogMock logMock = new LogMock();
            EasyIoc.IocContainer.Default.RegisterInstance<IClientBL>(new ClientBL());
            EasyIoc.IocContainer.Default.RegisterInstance<IClientDL>(clientDLMock);
            EasyIoc.IocContainer.Default.RegisterInstance<IMailSender.IMailSender>(mailSenderMock);
            EasyIoc.IocContainer.Default.RegisterInstance<ILog>(logMock);
            MailAutomationBL bl = new MailAutomationBL();
            clientDLMock.AddClient(new Client
            {
                Id = Guid.NewGuid(),
                ClientId = 1,
                FirstName = "Test",
                BirthDate = DateTime.Today.AddYears(-10).AddDays(-20),
                Email = "pouet@brol.net"
            });

            bl.SendAutomatedMailsAsync().Wait();

            Assert.IsTrue(mailSenderMock.Mails.Count == 0);
        }

        [TestMethod]
        public void TestClientBirthDayIsTodayAndAlreadyWished()
        {
            ClientDLMock clientDLMock = new ClientDLMock();
            MailSenderMock mailSenderMock = new MailSenderMock();
            LogMock logMock = new LogMock();
            EasyIoc.IocContainer.Default.RegisterInstance<IClientBL>(new ClientBL());
            EasyIoc.IocContainer.Default.RegisterInstance<IClientDL>(clientDLMock);
            EasyIoc.IocContainer.Default.RegisterInstance<IMailSender.IMailSender>(mailSenderMock);
            EasyIoc.IocContainer.Default.RegisterInstance<ILog>(logMock);
            MailAutomationBL bl = new MailAutomationBL();
            clientDLMock.AddClient(new Client
            {
                Id = Guid.NewGuid(),
                ClientId = 1,
                FirstName = "Test",
                BirthDate = DateTime.Today.AddYears(-10),
                Email = "pouet@brol.net"
            });

            bl.SendAutomatedMailsAsync().Wait();
            bl.SendAutomatedMailsAsync().Wait();

            Assert.IsTrue(mailSenderMock.Mails.Count == 1);
            Assert.IsTrue(mailSenderMock.Mails.Single().Contains("BIRTHDAY_"));
        }

        [TestMethod]
        public void TestClientBirthDayWasYesterdayAndWished()
        {
            ClientDLMock clientDLMock = new ClientDLMock();
            MailSenderMock mailSenderMock = new MailSenderMock();
            LogMock logMock = new LogMock();
            EasyIoc.IocContainer.Default.RegisterInstance<IClientBL>(new ClientBL());
            EasyIoc.IocContainer.Default.RegisterInstance<IClientDL>(clientDLMock);
            EasyIoc.IocContainer.Default.RegisterInstance<IMailSender.IMailSender>(mailSenderMock);
            EasyIoc.IocContainer.Default.RegisterInstance<ILog>(logMock);
            MailAutomationBL bl = new MailAutomationBL();
            clientDLMock.AddClient(new Client
            {
                Id = Guid.NewGuid(),
                ClientId = 1,
                FirstName = "Test",
                BirthDate = DateTime.Today.AddYears(-10).AddDays(-1),
                LastBirthMailDate = DateTime.Today,
                Email = "pouet@brol.net"
            });

            bl.SendAutomatedMailsAsync().Wait();

            Assert.IsTrue(mailSenderMock.Mails.Count == 0);
        }
    }

    public class ClientDLMock : IClientDL
    {
        private readonly List<Client> _clients = new List<Client>();

        public void AddClient(Client client)
        {
            _clients.Add(client);
        }

        public List<Client> GetClients()
        {
            return _clients.ToList();
        }

        public Client GetClient(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<Client> SearchClients(string firstNameFilter, string lastNameFilter)
        {
            throw new NotImplementedException();
        }

        public List<Client> SearchClients(string filter)
        {
            throw new NotImplementedException();
        }

        public void SaveClient(Client client)
        {
            throw new NotImplementedException();
        }

        public void SavePurchase(Client client, Purchase purchase)
        {
            throw new NotImplementedException();
        }

        public int GetMaxClientId()
        {
            throw new NotImplementedException();
        }

        public List<Client> GetClients(Func<Client, bool> filterFunc)
        {
            return _clients.Where(filterFunc).ToList();
        }

        public void DeleteClient(Client client)
        {
            throw new NotImplementedException();
        }

        // Statistics

        public BestClient GetBestClientInPeriod(DateTime @from, DateTime till)
        {
            throw new NotImplementedException();
        }

        public Dictionary<AgeCategories, int> GetClientCountByAgeCategory()
        {
            throw new NotImplementedException();
        }

        public Dictionary<Sex, int> GetClientCountBySex()
        {
            throw new NotImplementedException();
        }

        public Dictionary<AgeCategories, decimal> GetClientAverageAmountByAgeCategory()
        {
            throw new NotImplementedException();
        }

        public FooterInformations GetFooterInformations()
        {
            throw new NotImplementedException();
        }
    }

    public class MailSenderMock : IMailSender.IMailSender
    {
        public List<string> Mails { get; } = new List<string>();

        public async Task SendHappyBirthdayMailAsync(string recipientMail, string firstName, DateTime? birthDate)
        {
            Mails.Add($"BIRTHDAY_{firstName}");
            await Task.Delay(0);
        }

        public async Task SendNewClientMailAsync(string recipientMail, string firstName)
        {
            Mails.Add($"WELCOME_{firstName}");
            await Task.Delay(0);
        }

        public async Task SendVoucherMailAsync(string recipientMail, string firstName, decimal discount)
        {
            Mails.Add($"VOUCHER_{firstName}");
            await Task.Delay(0);
        }
    }

    public class LogMock : ILog
    {
        public List<string> DebugMessages { get; } = new List<string>();
        public List<string> InfoMessages { get; } = new List<string>();
        public List<string> WarningMessages { get; } = new List<string>();
        public List<string> ErrorMessages { get; } = new List<string>();
        public List<string> ExceptionMessages { get; } = new List<string>();
        public List<string> WriteLineMessages { get; } = new List<string>();

        public void Initialize(string path, string file, string fileTarget = "logfile")
        {
        }

        public void Debug(string format, params object[] args)
        {
            DebugMessages.Add(string.Format(format, args));
        }

        public void Info(string format, params object[] args)
        {
            InfoMessages.Add(string.Format(format, args));
        }

        public void Warning(string format, params object[] args)
        {
            WarningMessages.Add(string.Format(format, args));
        }

        public void Error(string format, params object[] args)
        {
            ErrorMessages.Add(string.Format(format, args));
        }

        public void Exception(Exception ex)
        {
            ExceptionMessages.Add(ex.ToString());
        }

        public void Exception(string msg, Exception ex)
        {
            ExceptionMessages.Add(msg + Environment.NewLine + ex.ToString());
        }

        public void WriteLine(LogLevels level, string format, params object[] args)
        {
            WriteLineMessages.Add(level + ":" + string.Format(format, args));
        }
    }
}