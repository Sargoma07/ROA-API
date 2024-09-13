using ROA.Rest.API.Domain.Statuses;

namespace ROA.Rest.API.Models;

public class PaymentModel
{
    public string? Id { get; set; }
    public required string CustomerId { get; set; }
    public required string MerchantId { get; set; }
    public PaymentStatus Status { get; set; }

    public OrderModel Order { get; set; } = new();
    public TotalDetailsModel TotalDetails { get; set; } = new();
    
    public record OrderModel
    {
        public IList<OrderLineModel> Lines { get; set; } = new List<OrderLineModel>();
    }
    
    public record OrderLineModel
    {
        public int Count { get; set; }
        public required string DataSpec { get; set; }
        public decimal PricePerUnit { get; set; }
    }
    
    public record TotalDetailsModel
    {
        public decimal Total { get; set; }
    }
}