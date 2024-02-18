using ROA.Model.Contract;

namespace ROA.Model;

public class Player: IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
    public required string ExternalId { get; set; }
    public required string Provider { get; set; }
    public decimal AmountAccount { get; set; }

    public void AddAmount(decimal amount)
    {
        AmountAccount += amount;
    }

    public static Player Create(string externalId, string provider)
    {
        return new Player()
        {
            ExternalId = externalId,
            Provider = provider,
            AmountAccount = 0m
        };
    }
}