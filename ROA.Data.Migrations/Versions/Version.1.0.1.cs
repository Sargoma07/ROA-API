using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBMigrations;

namespace ROA.Data.Migrations.Versions;

public class Version101 : IMigration
{
    public MongoDBMigrations.Version Version => new(1, 0, 1);

    public string Name => "Version 1.0.1";

    private const string ETagField = "ETag";
    
    private const string IdField = "_id";
    private const string PlayerId = "632468860d43ec833dcbb83e";

    private const string InventoryId = "632468860d43ec833dcbb831";
    private const string StorageId = "632468860d43ec833dcbb832";
    private const string EquipmentId = "632468860d43ec833dcbb833";

    private const string ArmorSpec =
        "/Script/Engine.BlueprintGeneratedClass'/Game/Gameplay/Inventory/InventoryItemData/BP_ArmorData.BP_ArmorData_C'";

    private const string TestGoldSpec =
        "/Script/Engine.BlueprintGeneratedClass'/Game/Gameplay/Inventory/InventoryItemData/BP_GoldInventoryData.BP_GoldInventoryData_C'";


    public void Up(IMongoDatabase database)
    {
        Console.WriteLine("Updating collections");

        UpPlayer(database);

        UpInventory(database);

        Console.WriteLine("Migration completed");
    }

    private static void UpPlayer(IMongoDatabase database)
    {
        var playerCollection = database.GetCollection<BsonDocument>("Player");

        var playerDocument = new BsonDocument()
        {
            { IdField, PlayerId },
            {ETagField, Guid.NewGuid()},
            { "_t", "Player" },
            { "ExternalId", "TestPlayerId" },
            { "Provider", "Local" }
        };
        playerCollection.InsertOne(playerDocument);
    }

    private static void UpInventory(IMongoDatabase database)
    {
        var inventoryCollection = database.GetCollection<BsonDocument>("Inventory");

        var slots = GenerateEmptySlots(40).ToList();
        slots[0] = CreateInventoryDocument("0", ArmorSpec, 1);
        slots[1] = CreateInventoryDocument("1", TestGoldSpec, 10);

        var inventoryDocument = new BsonDocument()
        {
            { IdField, InventoryId },
            {ETagField, Guid.NewGuid()},
            { "_t", "Inventory" },
            { "PlayerId", PlayerId },
            { "Type", "CharacterInventory" },
            {
                "Slots", new BsonArray(slots)
            }
        };

        inventoryCollection.InsertOne(inventoryDocument);
        
        var storageDocument = new BsonDocument()
        {
            { IdField, StorageId },
            {ETagField, Guid.NewGuid()},
            { "_t", "Inventory" },
            { "PlayerId", PlayerId },
            { "Type", "Storage" },
            {
                "Slots", new BsonArray(GenerateEmptySlots(72))
            }
        };

        inventoryCollection.InsertOne(storageDocument);
        
        var equipmentDocument = new BsonDocument()
        {
            { IdField, EquipmentId },
            {ETagField, Guid.NewGuid()},
            { "_t", "Inventory" },
            { "PlayerId", PlayerId },
            { "Type", "Equipment" },
            {
                "Slots", new BsonArray()
            }
        };

        inventoryCollection.InsertOne(equipmentDocument);
    }

    public void Down(IMongoDatabase database)
    {
        Console.WriteLine("Updating collections");

        var playerCollection = database.GetCollection<BsonDocument>("Player");
        playerCollection.DeleteOne(x => x[IdField] == PlayerId);
        
        var inventoryCollection = database.GetCollection<BsonDocument>("Inventory");
        inventoryCollection.DeleteOne(x => x[IdField] == InventoryId);
        inventoryCollection.DeleteOne(x => x[IdField] == StorageId);
        inventoryCollection.DeleteOne(x => x[IdField] == EquipmentId);

        Console.WriteLine("Migration completed");
    }

    private static IEnumerable<BsonDocument> GenerateEmptySlots(int count)
    {
        var slots = new List<BsonDocument>(count);
        for (var i = 0; i < count; i++)
        {
            var document = CreateInventoryDocument(i.ToString());

            slots.Add(document);
        }

        return slots;
    }

    private static BsonDocument CreateInventoryDocument(string slotKey, string dataSpec = "None", int count = 0)
    {
        return new BsonDocument()
        {
            { "Slot", slotKey },
            {
                "Data", new BsonDocument()
                {
                    { "Count", count },
                    { "DataSpec", dataSpec }
                }
            }
        };
    }
}