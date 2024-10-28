using ROA.Infrastructure.Domain;
using ROA.Payment.API.Domain.Statuses;

namespace ROA.Payment.API.Domain;

public class Payment : IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
    public required string CustomerId { get; set; }
    public required string MerchantId { get; set; }
    public required string AccountId { get; set; }
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
        public required string Name { get; set; }
        public int Count { get; set; }
        public decimal PricePerUnit { get; set; }
        public required string Currency { get; set; }
    }

    public record PaymentAmountDetails
    {
        public decimal Amount { get; set; }  = 0.0m;
        public  string Currency { get; set; } = string.Empty;
    }

    public record PaymentTotalDetails
    {
        public decimal Total { get; set; } = 0.0m;  
        public  string Currency { get; set; } = string.Empty;
    }

}