namespace ROA.Shop.API.Models;

public class ItemPriceModel
{
    public string Id { get; set; }
    public required string UniqueName { get; set; }
    public decimal Price { get; set; }
    public required string Currency { get; set; }
}