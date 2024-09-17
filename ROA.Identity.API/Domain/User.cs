using ROA.Infrastructure.Domain;

namespace ROA.Identity.API.Domain;

public class User : IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
    public required string ExternalId { get; set; }
    public required string Provider { get; set; }

    public IDictionary<string, RefreshTokenSession> RefreshTokenSessions { get; set; } =
        new Dictionary<string, RefreshTokenSession>();

    public record RefreshTokenSession
    {
        public required string AccessToken { get; set; }
        public required DateTime RefreshExpires { get; set; }
    }
}