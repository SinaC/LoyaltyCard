using System.Windows;
using Loyalty.DataAccess.FileBased;
using Loyalty.IBusiness;
using Loyalty.IDataAccess;
using LoyaltyCard.Business;

namespace Loyalty.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            EasyIoc.IocContainer.Default.RegisterInstance<IClientDL>(new ClientDL());
            EasyIoc.IocContainer.Default.RegisterInstance<IClientBL>(new ClientBL());

            MainViewModel vm = new MainViewModel();
            DataContext = vm;
        }
    }
}
