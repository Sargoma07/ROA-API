namespace ROA.Rest.API.Models;

public class PlayerSignUpModel
{
    public required string ExternalId { get; set; }
    public required string Provider { get; set; }
}