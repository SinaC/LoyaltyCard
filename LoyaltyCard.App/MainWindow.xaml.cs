using System;
using System.Threading.Tasks;
using System.Windows;
using LoyaltyCard.IBusiness;
using LoyaltyCard.Log;
using LoyaltyCard.Services.Popup;

namespace LoyaltyCard.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Register Popup mainWindow
            EasyIoc.IocContainer.Default.RegisterInstance<IPopupService>(new PopupService(this));

            //
            MainViewModel vm = new MainViewModel();
            DataContext = vm;

            // Send mails
            Task.Run(SendAutomatedMails);
        }

        private async Task SendAutomatedMails()
        {
            ILog logger = EasyIoc.IocContainer.Default.Resolve<ILog>();
            try
            {
                await EasyIoc.IocContainer.Default.Resolve<IMailAutomationBL>().SendAutomatedMailsAsync();
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
            }
        }
    }
}
