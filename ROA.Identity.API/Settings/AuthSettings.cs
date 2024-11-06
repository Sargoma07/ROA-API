namespace ROA.Identity.API.Settings;

public record AuthSettings
{
    public required string Secret { get; init; }

    public int TokenExpireMinutes { get; init; }

    public int RefreshExpiresDays { get; init; }

    public required string Issuer { get; init; }

    public required string Audience { get; init; }
}