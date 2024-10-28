using ROA.Infrastructure.Domain;

namespace ROA.Shop.API.Domain;

public class ItemPrice : IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
    public required string UniqueName { get; set; }
    public IEnumerable<PriceDetail> Details { get; set; } = new List<PriceDetail>();

    public record PriceDetail
    {
        public required string Currency { get; set; } 
        public decimal Price { get; set; }
    }
}