using ROA.Rest.API.Domain.Statuses;

namespace ROA.Rest.API.Domain;

public class Payment : IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
    public required string CustomerId { get; set; }
    public required string MerchantId { get; set; }
    public PaymentStatus Status { get; set; }

    public PaymentOrder Order { get; set; } = new();

    public PaymentTotalDetails TotalDetails { get; set; } = new();

    public PaymentAmountDetails AmountDetails { get; set; } = new();

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }


    public record PaymentOrder
    {
        public IList<OrderLine> Lines { get; set; } = new List<OrderLine>();
    }

    public record OrderLine
    {
        public int Count { get; set; }
        public required string DataSpec { get; set; }
        public decimal PricePerUnit { get; set; }
    }

    public record PaymentAmountDetails
    {
        public decimal Amount { get; set; }
    }

    public record PaymentTotalDetails
    {
        public decimal Total { get; set; }
    }

}