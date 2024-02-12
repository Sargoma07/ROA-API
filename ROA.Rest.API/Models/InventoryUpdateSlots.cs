using ROA.Model;

namespace ROA.Rest.API.Models;

public class InventoryUpdateSlots
{
    public IList<InventoryItemSlot> Slots { get; set; } = new List<InventoryItemSlot>();
}