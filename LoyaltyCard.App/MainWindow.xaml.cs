using System.Windows;
using LoyaltyCard.IBusiness;
using LoyaltyCard.IDataAccess;
using LoyaltyCard.Business;
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

            EasyIoc.IocContainer.Default.RegisterInstance<IClientDL>(new DataAccess.FileBased.ClientDL());
            EasyIoc.IocContainer.Default.RegisterInstance<IClientBL>(new ClientBL());
            EasyIoc.IocContainer.Default.RegisterInstance<IGeoDL>(new DataAccess.FileBased.GeoDL());
            EasyIoc.IocContainer.Default.RegisterInstance<IGeoBL>(new GeoBL());
            EasyIoc.IocContainer.Default.RegisterInstance<IPopupService>(new PopupService(this));

            MainViewModel vm = new MainViewModel();
            DataContext = vm;
        }
    }
}
