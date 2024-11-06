namespace ROA.Payment.API.Models;

public record AccountModel
{
    public IList<BalanceDataModel> Balances { get; init; } = new List<BalanceDataModel>();

    public record BalanceDataModel
    {
        public required string Currency { get; init; }
        public decimal Amount { get; init; }
    }
}