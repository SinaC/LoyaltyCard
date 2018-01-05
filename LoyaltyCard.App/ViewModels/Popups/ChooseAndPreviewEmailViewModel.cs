using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using EasyMVVM;
using LoyaltyCard.App.Views.Popups;
using LoyaltyCard.Common.Extensions;
using LoyaltyCard.Services.IO;
using LoyaltyCard.Services.Popup;

namespace LoyaltyCard.App.ViewModels.Popups
{
    [PopupAssociatedView(typeof(ChooseAndPreviewEmailView))]
    public class ChooseAndPreviewEmailViewModel : ViewModelBase
    {
        #region  Email content

        private string _emailContent;
        public string EmailContent
        {
            get { return _emailContent; }
            protected set { Set(() => EmailContent, ref _emailContent, value); }
        }

        #endregion

        #region Choose email

        private ICommand _chooseEmailCommand;
        public ICommand ChooseEmailCommand => _chooseEmailCommand ?? (_chooseEmailCommand = new RelayCommand(ChooseEmail));
        private void ChooseEmail()
        {
            IIOService ioService = EasyIoc.IocContainer.Default.Resolve<IIOService>();
            string mailsPath = ConfigurationManager.AppSettings["MailTemplatesPath"];
            string filePath = ioService.OpenFileDialog(mailsPath, "html", "html files (*.html)|*.html");
            try
            {
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    string html = File.ReadAllText(filePath);
                    if (!string.IsNullOrWhiteSpace(html))
                    {
                        html = html.Replace("\t", string.Empty).Replace(Environment.NewLine, string.Empty);
                        // Remove cid:[tag] with tag related filename
                        Regex pattern = new Regex(@"img src=""(?<cid>cid\:\w+)""");
                        EmailContent = pattern.ReplaceGroup(html, "cid", ReplaceCid);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private string ReplaceCid(string cid)
        {
            if (!string.IsNullOrWhiteSpace(cid))
            {
                string imageSource = cid.Replace("cid:", string.Empty);
                string picturesPath = ConfigurationManager.AppSettings["MailPicturesPath"];
                string imagePath = Directory.EnumerateFiles(picturesPath, imageSource + ".*").FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(imagePath))
                {
                    return imagePath;
                }
            }
            return string.Empty;
        }

        #endregion

        #region Send mail

        private ICommand _sendEmailCommand;
        public ICommand SendEmailCommand => _sendEmailCommand ?? (_sendEmailCommand = new RelayCommand(SendEmail));

        private void SendEmail()
        {
            NotYetImplemented();
        }

#endregion
    }

    public class ChooseAndPreviewEmailViewModelDesignData : ChooseAndPreviewEmailViewModel
    {
    }
}
