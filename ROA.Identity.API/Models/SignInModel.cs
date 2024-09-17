namespace ROA.Identity.API.Models;

public class SignInModel
{
    public required string ExternalId { get; set; } 
    public required string Provider { get; set; } 
}