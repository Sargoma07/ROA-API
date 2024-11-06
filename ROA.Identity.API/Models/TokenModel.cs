namespace ROA.Identity.API.Models;

public record TokenModel
{
    public required string Access { get; init; } 
    public required string Refresh { get; init; }
}