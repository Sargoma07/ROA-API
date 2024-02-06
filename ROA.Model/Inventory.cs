using ROA.Model.Contract;

namespace ROA.Model;

public class Inventory: IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
}