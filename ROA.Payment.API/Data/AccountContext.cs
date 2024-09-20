namespace ROA.Payment.API.Data;

public interface IAccountContext
{
    string AccountId { get; set; }
}

internal class AccountContext: IAccountContext
{
    public required string AccountId { get; set; }
}