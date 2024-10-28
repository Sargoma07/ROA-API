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

    public record InventoryItemSlot
    {
        public required string Slot { get; set; }
        public InventoryItem Data { get; set; }
    }

    public record InventoryItem
    {
        public int Count { get; set; }
        public required string DataSpec { get; set; }
    }
}