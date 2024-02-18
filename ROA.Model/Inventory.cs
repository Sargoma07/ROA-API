using ROA.Model.Contract;
using ROA.Model.Types;

namespace ROA.Model;

public class Inventory : IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
    public required string PlayerId { get; set; }
    public InventoryType Type { get; set; }
    public IList<InventoryItemSlot> Slots { get; set; } = new List<InventoryItemSlot>();

    public static Inventory CreateInventory(string playerId, IEnumerable<InventoryItemSlot> slots)
    {
        return new Inventory
        {
            PlayerId = playerId,
            Slots = slots.ToList(),
            Type = InventoryType.CharacterInventory
        };
    }
    
    public static Inventory CreateStorage(string playerId, IEnumerable<InventoryItemSlot> slots)
    {
        return new Inventory
        {
            PlayerId = playerId,
            Slots = slots.ToList(),
            Type = InventoryType.Storage
        };
    }
    
    public static Inventory CreateEquipment(string playerId, IEnumerable<InventoryItemSlot> slots)
    {
        return new Inventory
        {
            PlayerId = playerId,
            Slots = slots.ToList(),
            Type = InventoryType.Equipment
        };
    }
}