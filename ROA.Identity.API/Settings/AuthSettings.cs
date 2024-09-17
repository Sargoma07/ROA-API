namespace ROA.Identity.API.Settings;

public class AuthSettings
{
    public required string Secret { get; set; }

    public int TokenExpireMinutes { get; set; }

    public int RefreshExpiresDays { get; set; }

    public required string Issuer { get; set; }

    public required string Audience { get; set; }
}