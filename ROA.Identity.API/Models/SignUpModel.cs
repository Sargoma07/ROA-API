namespace ROA.Identity.API.Models;

public record SignUpModel
{
    public required string ExternalId { get; init; }
    public required string Provider { get; init; }
}