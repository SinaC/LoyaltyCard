using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LoyaltyCard.MailSender
{
    // TODO: better error handling + log
    public class MailSender : IMailSender.IMailSender
    {
        private static IDictionary<string, string> _config = new Dictionary<string, string>();

        //https://stackoverflow.com/questions/2317012/attaching-image-in-the-body-of-mail-in-c-sharp
        //http://www.theukwebdesigncompany.com/articles/entity-escape-characters.php

        private MailMessage BuildMailMessage(MailAddress fromAddress, MailAddress toAddress, MailAddress replyToAddress, string subject, string mailTemplate, IReadOnlyDictionary<string, string> replacements)
        {
            MailMessage msg = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                ReplyToList =
                {
                    replyToAddress
                },
                IsBodyHtml = true
            };

            string path = Path.Combine(ConfigurationManager.AppSettings["MailTemplatesPath"], mailTemplate);
            string body = File.ReadAllText(path).Replace("\t", string.Empty).Replace(Environment.NewLine, string.Empty);

            // Parse and search img, then create attachment for each image     cid will be used as filename (without ext)
            //<img src="cid:XXX"
            Regex pattern = new Regex(@"img src=""cid:(?<cid>\w+)""");
            MatchCollection myMatches = pattern.Matches(body);

            // For each cid, search related picture and add as attachement
            foreach (Match match in myMatches)
            {
                string cid = match.Groups["cid"].Value;
                if (!string.IsNullOrWhiteSpace(cid))
                {
                    string picturesPath = ConfigurationManager.AppSettings["MailPicturesPath"];
                    string imagePath = Directory.EnumerateFiles(picturesPath, cid + ".*").FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(imagePath))
                    {
                        string imageType = Path.GetExtension(imagePath).Substring(1); // get extension and remove .
                        Attachment attachment = new Attachment(imagePath);
                        attachment.ContentDisposition.Inline = true;
                        attachment.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        attachment.ContentId = cid;
                        attachment.ContentType.MediaType = "image/" + imageType;
                        attachment.ContentType.Name = Path.GetFileName(imagePath);
                        msg.Attachments.Add(attachment);
                    }
                }
            }

            // Perform replacements if any
            if (replacements != null)
            {
                foreach (KeyValuePair<string, string>kv in replacements)
                    body = body.Replace(kv.Key, kv.Value);
            }

            msg.Body = body;

            return msg;
        }

        public async Task SendHappyBirthdayMailAsync(string recipientMail, string firstName, DateTime? birthDate)
        {
            string senderMail = GetConfigValue("SenderMail");
            string senderPassword = GetConfigValue("SenderPassword");
            string replyToMail = GetConfigValue("ReplyToMail");

            MailAddress fromAddress = new MailAddress(senderMail, "PPC SPRL");
            MailAddress toAddress = new MailAddress(recipientMail, firstName ?? "vous");
            MailAddress replyToAddress = new MailAddress(replyToMail, "PPC SPRL");

            DateTime maxValidity = (birthDate ?? DateTime.Today).AddMonths(1);

            using (SmtpClient client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, senderPassword)
            })
            {
                using (var message = BuildMailMessage(fromAddress, toAddress, replyToAddress, "Joyeux anniversaire de la part de l'équipe PPC", "birthday.html", new Dictionary<string, string>
                {
                    {"[firstname]", string.IsNullOrWhiteSpace(firstName) ? "client" : firstName},
                    {"[maxvalidity]", $"{maxValidity:dd/MM/yyyy}"}
                }))
                {
                    await client.SendMailAsync(message);
                }
            }
        }

        public async Task SendNewClientMailAsync(string recipientMail, string firstName)
        {
            string senderMail = GetConfigValue("SenderMail");
            string senderPassword = GetConfigValue("SenderPassword");
            string replyToMail = GetConfigValue("ReplyToMail");

            MailAddress fromAddress = new MailAddress(senderMail, "PPC SPRL");
            MailAddress toAddress = new MailAddress(recipientMail, firstName ?? "vous");
            MailAddress replyToAddress = new MailAddress(replyToMail, "PPC SPRL");

            using (SmtpClient client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, senderPassword)
            })
            {
                using (var message = BuildMailMessage(fromAddress, toAddress, replyToAddress, "Félicitations de la part de l'équipe PPC", "newclient.html", new Dictionary<string, string>
                {
                    {"[firstname]", string.IsNullOrWhiteSpace(firstName) ? "client" : firstName},
                }))
                {
                    await client.SendMailAsync(message);
                }
            }
        }

        public async Task SendVoucherMailAsync(string recipientMail, string firstName, decimal discount)
        {
            string senderMail = GetConfigValue("SenderMail");
            string senderPassword = GetConfigValue("SenderPassword");
            string replyToMail = GetConfigValue("ReplyToMail");

            MailAddress fromAddress = new MailAddress(senderMail, "PPC SPRL");
            MailAddress toAddress = new MailAddress(recipientMail, firstName ?? "vous");
            MailAddress replyToAddress = new MailAddress(replyToMail, "PPC SPRL");

            using (SmtpClient client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, senderPassword)
            })
            {
                using (var message = BuildMailMessage(fromAddress, toAddress, replyToAddress, "Bon de réduction chez PPC", "voucher.html", new Dictionary<string, string>
                {
                    {"[firstname]", string.IsNullOrWhiteSpace(firstName) ? "client" : firstName},
                    {"[discount]", $"{discount}%" }
                }))
                {
                    await client.SendMailAsync(message);
                }
            }
        }

        private string GetConfigValue(string key) // TODO: not thread safe
        {
            string value;
            if (_config.TryGetValue(key, out value))
                return value;

            string configFileName = ConfigurationManager.AppSettings["ConfigFile"];
            XDocument document = XDocument.Load(configFileName);
            _config = document.Root?.Elements("add").ToDictionary(e => e.Attribute("key")?.Value, e => e.Attribute("value")?.Value);

            if (_config?.TryGetValue(key, out value) == true)
                return value;
            return null;
        }
    }
}
