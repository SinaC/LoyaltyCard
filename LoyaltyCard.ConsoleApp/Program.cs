using LoyaltyCard.DataAccess.FileBased;
using LoyaltyCard.IBusiness;
using LoyaltyCard.IDataAccess;
using LoyaltyCard.Business;

namespace LoyaltyCard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            EasyIoc.IocContainer.Default.RegisterInstance<IClientDL>(new ClientDL());
            EasyIoc.IocContainer.Default.RegisterInstance<IClientBL>(new ClientBL());

            //Client client1 = EasyIoc.IocContainer.Default.Resolve<IClientBL>().GetClients().FirstOrDefault(x => x.LastName == "Pouet")
            //                 ?? new Client
            //                 {
            //                     FirstName = "Toto",
            //                     LastName = "Pouet",
            //                     BirthDate = new DateTime(1970, 01, 01),
            //                     Email = "pouet.brol@gmail.com",
            //                     Mobile = null,
            //                 };
            //EasyIoc.IocContainer.Default.Resolve<IClientBL>().SaveClient(client1);

            //client1.BirthDate = new DateTime(1970, 01, 01),
            //EasyIoc.IocContainer.Default.Resolve<IClientBL>().SaveClient(client1);
        }
    }
}
