using ROA.Payment.API.Domain.Statuses;

namespace ROA.Payment.API.Models;

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
        public required string Name { get; set; }
        public int Count { get; set; }
        public decimal PricePerUnit { get; set; }
        public required string Currency { get; set; }
    }

    public record TotalDetailsModel
    {
        public decimal Total { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}