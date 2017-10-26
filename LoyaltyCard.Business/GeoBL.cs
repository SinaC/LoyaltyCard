using LoyaltyCard.IBusiness;
using LoyaltyCard.IDataAccess;

namespace LoyaltyCard.Business
{
    public class GeoBL : IGeoBL
    {
        private IGeoDL GeoDL => EasyIoc.IocContainer.Default.Resolve<IGeoDL>();

        public string GetCityFromZip(string zip)
        {
            string cityFromDL = GeoDL.GetCityFromZip(zip);
            if (!string.IsNullOrWhiteSpace(cityFromDL))
                return cityFromDL;
            // TODO: gather from GeoNames
            return null;
        }
    }
}
