namespace ROA.Identity.API.Domain.Dtos;

public record AuthDataDto
{
    public required string ExternalId { get; init; }
}