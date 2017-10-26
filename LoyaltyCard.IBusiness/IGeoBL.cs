namespace LoyaltyCard.IBusiness
{
    public interface IGeoBL
    {
        string GetCityFromZip(string zip);
        // TODO: get GPS coordinates for whole address
    }
}
