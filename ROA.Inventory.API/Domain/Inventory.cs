using ROA.Infrastructure.Domain;
using ROA.Inventory.API.Domain.Types;

namespace ROA.Inventory.API.Domain;

public class Inventory : IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
    public required string PlayerId { get; set; }
    public InventoryType Type { get; set; }
    public IList<InventoryItemSlot> Slots { get; set; } = new List<InventoryItemSlot>();

    // ReSharper disable once ClassNeverInstantiated.Global
    public class InventoryItemSlot
    {
        public required string Slot { get; set; }
        public required InventoryItem Data { get; set; }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class InventoryItem
    {
        public int Count { get; set; }
        public required string DataSpec { get; set; }
    }
}