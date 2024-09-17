
namespace ROA.Identity.API.Models;

public class RefreshTokenModel
{
    public string Access { get; set; } = string.Empty;
    public string Refresh { get; set; } =  string.Empty;
}