using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loyalty.DataAccess.FileBased;
using Loyalty.IBusiness;
using Loyalty.IDataAccess;
using LoyaltyCard.Business;
using LoyaltyCard.Domain;

namespace LoyaltyCard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            EasyIoc.IocContainer.Default.RegisterInstance<IClientDL>(new ClientDL());
            EasyIoc.IocContainer.Default.RegisterInstance<IClientBL>(new ClientBL());

            //Client client1 = EasyIoc.IocContainer.Default.Resolve<IClientBL>().GetClients().FirstOrDefault(x => x.LastName == "Heymbeeck")
            //                 ?? new Client
            //                 {
            //                     FirstName = "Joel",
            //                     LastName = "Heymbeeck",
            //                     BirthDate = new DateTime(1976, 12, 01),
            //                     Email = "sinac_be@yahoo.fr",
            //                     Mobile = null,
            //                 };
            //EasyIoc.IocContainer.Default.Resolve<IClientBL>().SaveClient(client1);

            //client1.BirthDate = new DateTime(1976, 12, 07);
            //EasyIoc.IocContainer.Default.Resolve<IClientBL>().SaveClient(client1);
        }
    }
}
