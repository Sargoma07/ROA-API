using ROA.Model;

namespace ROA.Rest.API.Models;

public class InventoryUpdateSlotsModel
{
    public IList<InventoryItemSlot> Slots { get; set; } = new List<InventoryItemSlot>();
}