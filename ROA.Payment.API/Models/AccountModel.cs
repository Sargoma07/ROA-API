namespace ROA.Payment.API.Models;

public class AccountModel
{
    public IList<BalanceDataModel> Balances { get; set; } = new List<BalanceDataModel>();

    public record BalanceDataModel
    {
        public required string Currency { get; set; }
        public decimal Amount { get; set; }
    }
}