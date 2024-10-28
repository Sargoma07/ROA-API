namespace ROA.Payment.API.ServiceAPI.ShopService;

public class ItemPriceModel
{
    public required string Id { get; set; }
    public required string UniqueName { get; set; }
    public decimal Price { get; set; }
    public required string Currency { get; set; }
}