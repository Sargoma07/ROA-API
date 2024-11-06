
namespace ROA.Identity.API.Models;

public record RefreshTokenModel
{
    public string Access { get; init; } = string.Empty;
    public string Refresh { get; init; } =  string.Empty;
}