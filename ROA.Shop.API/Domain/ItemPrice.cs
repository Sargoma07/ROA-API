using ROA.Infrastructure.Domain;

namespace ROA.Shop.API.Domain;

// ReSharper disable once ClassNeverInstantiated.Global
public class ItemPrice : IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
    public required string UniqueName { get; set; }
    public IEnumerable<PriceDetail> Details { get; set; } = new List<PriceDetail>();

    // ReSharper disable once ClassNeverInstantiated.Global
    public class PriceDetail
    {
        public required string Currency { get; set; } 
        public decimal Price { get; set; }
    }
}