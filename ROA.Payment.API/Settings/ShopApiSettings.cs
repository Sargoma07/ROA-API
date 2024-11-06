namespace ROA.Payment.API.Settings;

public record ShopApiSettings
{
    public required string BaseAddress { get; init; }
}