using ROA.Model.Contract;

namespace ROA.Model;

public class Player: IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
    public string ExternalId { get; set; }
    public string Provider { get; set; }

    public static Player Create(string externalId, string provider)
    {
        return new Player()
        {
            ExternalId = externalId,
            Provider = provider
        };
    }
}