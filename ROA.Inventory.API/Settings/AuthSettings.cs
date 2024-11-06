namespace ROA.Inventory.API.Settings;

public record AuthSettings
{
    public required string Secret { get; init; }

    public required string Issuer { get; init; }

    public required string Audience { get; init; }
}