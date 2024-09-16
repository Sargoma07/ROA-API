using ROA.Infrastructure.Domain;

namespace ROA.Rest.API.Domain;

public class ItemPrice : IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
    
    public IList<PriceDetail> PriceDetails { get; set; } = new List<PriceDetail>();

    public record PriceDetail
    {
        public required string DataSpec { get; set; }
        public decimal Price { get; set; }
    }
}