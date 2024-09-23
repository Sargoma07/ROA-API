namespace ROA.Shop.API.Models;

public class GetPricesRequest
{
    public IList<string> UniqueNames { get; set; } = new List<string>();
}