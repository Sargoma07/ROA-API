using ROA.Payment.API.Domain.Statuses;

namespace ROA.Payment.API.Models;

public record PaymentModel
{
    public string? Id { get; init; }
    public required string CustomerId { get; init; }
    public required string MerchantId { get; init; }
    public PaymentStatus Status { get; init; }

    public OrderModel Order { get; init; } = new();
    public TotalDetailsModel TotalDetails { get; init; } = new();

    public record OrderModel
    {
        public IList<OrderLineModel> Lines { get; init; } = new List<OrderLineModel>();
    }

    public record OrderLineModel
    {
        public required string Name { get; init; }
        public int Count { get; init; }
        public decimal PricePerUnit { get; init; }
        public required string Currency { get; init; }
    }

    public record TotalDetailsModel
    {
        public decimal Total { get; init; }
        public string Currency { get; init; } = string.Empty;
    }
}