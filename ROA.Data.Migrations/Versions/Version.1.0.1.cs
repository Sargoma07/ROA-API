using MongoDB.Bson;
using MongoDB.Driver;
using Version = MongoDBMigrations.Version;

namespace ROA.Data.Migrations.Versions;

public class Version101 : Migration
{
    public Version101()
    {
        Version = new Version(1, 0, 1);
        Name = "Version 1.0.1";
    }

    private const string ETagField = "ETag";
    
    private const string IdField = "_id";
    private const string UserId = "632468860d43ec833dcbb83e";

    private const string InventoryId = "632468860d43ec833dcbb831";
    private const string StorageId = "632468860d43ec833dcbb832";
    private const string EquipmentId = "632468860d43ec833dcbb833";

    private const string ArmorSpec =
        "/Script/Engine.BlueprintGeneratedClass'/Game/Gameplay/Inventory/InventoryItemData/BP_ArmorData.BP_ArmorData_C'";

    private const string TestGoldSpec =
        "/Script/Engine.BlueprintGeneratedClass'/Game/Gameplay/Inventory/InventoryItemData/BP_GoldInventoryData.BP_GoldInventoryData_C'";

    private const string GameGoldCurrency = "GAME_GOLD";


    protected override void UpExecute(IMongoDatabase database)
    {
        UpUser(database);
        
        UpAccount(database);

        UpInventory(database);
    }

    private static void UpUser(IMongoDatabase database)
    {
        var collection = database.GetCollection<BsonDocument>("User");

        var document = new BsonDocument
        {
            { IdField, UserId },
            { ETagField, Guid.NewGuid() },
            { "_t", "User" },
            { "ExternalId", "TestPlayerId" },
            { "Provider", "Local" }
        };

        collection.InsertOne(document);
    }

    private void UpAccount(IMongoDatabase database)
    {
        var collection = database.GetCollection<BsonDocument>("Account");

        var document = new BsonDocument
        {
            { IdField, UserId },
            { ETagField, Guid.NewGuid() },
            { "_t", "Account" },
            {
                "Balances", new BsonArray
                {
                    new BsonDocument
                    {
                        { "Currency", GameGoldCurrency },
                        { "Amount", 0.0m }
                    }
                }
            }
        };
        
        collection.InsertOne(document);
    }

    private static void UpInventory(IMongoDatabase database)
    {
        var collection = database.GetCollection<BsonDocument>("Inventory");

        var slots = GenerateEmptySlots(40).ToList();
        slots[0] = CreateInventoryDocument("0", ArmorSpec, 1);
        slots[1] = CreateInventoryDocument("1", TestGoldSpec, 10);

        var inventoryDocument = new BsonDocument()
        {
            { IdField, InventoryId },
            {ETagField, Guid.NewGuid()},
            { "_t", "Inventory" },
            { "PlayerId", UserId },
            { "Type", "CharacterInventory" },
            {
                "Slots", new BsonArray(slots)
            }
        };

        collection.InsertOne(inventoryDocument);
        
        var storageDocument = new BsonDocument()
        {
            { IdField, StorageId },
            {ETagField, Guid.NewGuid()},
            { "_t", "Inventory" },
            { "PlayerId", UserId },
            { "Type", "Storage" },
            {
                "Slots", new BsonArray(GenerateEmptySlots(72))
            }
        };

        collection.InsertOne(storageDocument);
        
        var equipmentDocument = new BsonDocument()
        {
            { IdField, EquipmentId },
            {ETagField, Guid.NewGuid()},
            { "_t", "Inventory" },
            { "PlayerId", UserId },
            { "Type", "Equipment" },
            {
                "Slots", new BsonArray()
            }
        };

        collection.InsertOne(equipmentDocument);
    }

    protected override void DownExecute(IMongoDatabase database)
    {
        var userCollection = database.GetCollection<BsonDocument>("User");
        userCollection.DeleteOne(x => x[IdField] == UserId);
        
        var accountCollection = database.GetCollection<BsonDocument>("Account");
        accountCollection.DeleteOne(x => x[IdField] == UserId);
        
        var inventoryCollection = database.GetCollection<BsonDocument>("Inventory");
        inventoryCollection.DeleteOne(x => x[IdField] == InventoryId);
        inventoryCollection.DeleteOne(x => x[IdField] == StorageId);
        inventoryCollection.DeleteOne(x => x[IdField] == EquipmentId);
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