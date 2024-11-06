namespace ROA.Inventory.API.Models;

public record InventoryModel
{
    public string? Id { get; init; }

    public IList<InventoryItemSlotModel> Slots { get; init; } = new List<InventoryItemSlotModel>();

    public record InventoryItemSlotModel
    {
        public required string Slot { get; init; }
        public required InventoryItemModel Data { get; init; }
    }

    public class InventoryItemModel
    {
        public int Count { get; init; }
        public required string DataSpec { get; init; }
    }
}