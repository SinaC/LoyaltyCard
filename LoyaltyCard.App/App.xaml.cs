using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
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
