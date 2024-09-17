namespace ROA.Identity.API.Models;

public class TokenModel
{
    public required string Access { get; set; } 
    public required string Refresh { get; set; }
}