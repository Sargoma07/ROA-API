namespace ROA.Identity.API.Models;

public class SignUpModel
{
    public required string ExternalId { get; set; }
    public required string Provider { get; set; }
}