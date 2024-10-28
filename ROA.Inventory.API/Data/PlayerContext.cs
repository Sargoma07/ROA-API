namespace ROA.Inventory.API.Data;

public interface IPlayerContext
{
    string PlayerId { get; set; }
}

internal class PlayerContext: IPlayerContext
{
    public required string PlayerId { get; set; }
}