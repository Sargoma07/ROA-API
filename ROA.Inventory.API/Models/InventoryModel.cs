namespace ROA.Inventory.API.Models;

public class InventoryModel
{
    public string? Id { get; set; }

    public IList<InventoryItemSlotModel> Slots { get; set; } = new List<InventoryItemSlotModel>();

    public record InventoryItemSlotModel
    {
        public required string Slot { get; set; }
        public InventoryItemModel? Data { get; set; }
    }

    public class InventoryItemModel
    {
        public int Count { get; set; }
        public required string DataSpec { get; set; }
    }
}