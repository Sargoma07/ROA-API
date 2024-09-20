using ROA.Infrastructure.Domain;

namespace ROA.Payment.API.Domain;

public class Account : IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
    public IList<BalanceData> Balances { get; set; } = new List<BalanceData>();
    
    public record BalanceData
    {
        public required string Currency { get; set; }
        public decimal Amount { get; set; }
    }

    public void AddAmount(string currency, decimal amount)
    {
        var balance = Balances.FirstOrDefault(x=>x.Currency == currency);

        if (balance is null)
        {
            throw new InvalidOperationException($"Balance not found with currency: {currency}");
        }
        
        balance.Amount += amount;
    }
}