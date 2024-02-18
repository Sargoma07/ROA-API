using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBMigrations;

namespace ROA.Data.Migrations.Versions;

public class Version102 : IMigration
{
    public MongoDBMigrations.Version Version => new(1, 0, 2);

    public string Name => "Version 1.0.2";

    private const string ETagField = "ETag";

    private const string IdField = "_id";
    private const string PriceId = "632468860d45ec833dcbb83e";

    private readonly Dictionary<string, decimal> _priceDetails = new()
    {
        {
            "/Script/Engine.BlueprintGeneratedClass'/Game/Gameplay/Inventory/InventoryItemData/BP_GoldInventoryData.BP_GoldInventoryData_C'",
            10m
        },
        {
            "/Script/Engine.BlueprintGeneratedClass'/Game/Gameplay/Inventory/InventoryItemData/BP_ArmorData.BP_ArmorData_C'",
            150m
        }
    };

    public void Up(IMongoDatabase database)
    {
        Console.WriteLine("Updating collections");

        var collection = database.GetCollection<BsonDocument>("ItemPrice");

        var playerDocument = new BsonDocument()
        {
            { IdField, PriceId },
            { ETagField, Guid.NewGuid() },
            { "_t", "ItemPrice" },
            { "PriceDetails", new BsonArray(GeneratePriceDetails(_priceDetails)) }
        };
        collection.InsertOne(playerDocument);

        Console.WriteLine("Migration completed");
    }

    public void Down(IMongoDatabase database)
    {
        Console.WriteLine("Updating collections");

        var collection = database.GetCollection<BsonDocument>("ItemPrice");
        collection.DeleteOne(x => x[IdField] == PriceId);

        Console.WriteLine("Migration completed");
    }

    private static IEnumerable<BsonDocument> GeneratePriceDetails(Dictionary<string, decimal> prices)
    {
        var slots = new List<BsonDocument>(prices.Count);
        slots.AddRange(prices.Select(price => new BsonDocument()
            {
                { "DataSpec", price.Key },
                { "Price", price.Value }
            }
        ));

        return slots;
    }
}