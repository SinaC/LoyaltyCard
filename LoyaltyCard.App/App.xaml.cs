using System;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using LoyaltyCard.Business;
using LoyaltyCard.IBusiness;
using LoyaltyCard.IDataAccess;
using LoyaltyCard.Log;

namespace LoyaltyCard.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ILog Logger => EasyIoc.IocContainer.Default.Resolve<ILog>();

        public App()
        {
            // Register instances
            EasyIoc.IocContainer.Default.RegisterInstance<ILog>(new NLogger());

            var clientDL = new DataAccess.FileBased.ClientDL(); // one DL for client, purchase and voucher
            EasyIoc.IocContainer.Default.RegisterInstance<IClientDL>(clientDL);
            EasyIoc.IocContainer.Default.RegisterInstance<IPurchaseDL>(clientDL);
            EasyIoc.IocContainer.Default.RegisterInstance<IVoucherDL>(clientDL);
            EasyIoc.IocContainer.Default.RegisterInstance<IGeoDL>(new DataAccess.FileBased.GeoDL());

            EasyIoc.IocContainer.Default.RegisterInstance<IClientBL>(new ClientBL());
            EasyIoc.IocContainer.Default.RegisterInstance<IPurchaseBL>(new PurchaseBL());
            EasyIoc.IocContainer.Default.RegisterInstance<IVoucherBL>(new VoucherBL());
            EasyIoc.IocContainer.Default.RegisterInstance<IGeoBL>(new GeoBL());
            EasyIoc.IocContainer.Default.RegisterInstance<IMailAutomationBL>(new MailAutomationBL());
            EasyIoc.IocContainer.Default.RegisterInstance<IStatisticsBL>(new StatisticsBL());

            EasyIoc.IocContainer.Default.RegisterInstance<IMailSender.IMailSender>(new MailSender.MailSender());

            // Backup DB
            clientDL.Backup();

            // Initialize log
            Logger.Initialize(ConfigurationManager.AppSettings["LogPath"], "${shortdate}.log");
            Logger.Info("Application started");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Exit += OnExit;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-BE");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-BE");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            //EventManager.RegisterClassHandler(typeof(WatermarkTextBox), UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(SelectivelyHandleMouseButton), true);
            //EventManager.RegisterClassHandler(typeof(WatermarkTextBox), UIElement.GotKeyboardFocusEvent, new RoutedEventHandler(SelectAllText), true);

            base.OnStartup(e);
        }

        private void OnExit(object sender, ExitEventArgs exitEventArgs)
        {
            Logger.Info("Application stopped");
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Logger.Exception("Unhandled Exception", unhandledExceptionEventArgs.ExceptionObject as Exception);
        }
    }
}
