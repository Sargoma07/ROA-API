using MongoDB.Bson;
using MongoDB.Driver;
using Version = MongoDBMigrations.Version;

namespace ROA.Data.Migrations.Versions;

public class Version102 : Migration
{
    public Version102()
    {
        Version = new Version(1, 0, 2);
        Name = "Version 1.0.2";
    }
    
    private const string ETagField = "ETag";

    private const string IdField = "_id";
    private const string ItemPriceId1 = "632468860d45ec833dcbb83e";
    private const string ItemPriceId2 = "632468860d45ec833dcbb84e";
    
    private const string GameGoldCurrency = "GAME_GOLD";

    protected override void UpExecute(IMongoDatabase database)
    {
        var collection = database.GetCollection<BsonDocument>("ItemPrice");

        var item1 = new BsonDocument()
        {
            { IdField, ItemPriceId1 },
            { ETagField, Guid.NewGuid() },
            { "_t", "ItemPrice" },
            { "UniqueName", "Gold_Test_ItemPrice" },
            { "Details", new BsonArray(CreatePriceDetails(GameGoldCurrency, 10)) }
        };
        collection.InsertOne(item1);
        
        var item2 = new BsonDocument()
        {
            { IdField, ItemPriceId2 },
            { ETagField, Guid.NewGuid() },
            { "_t", "ItemPrice" },
            { "UniqueName", "Armor_Test_ItemPrice" },
            { "Details", new BsonArray(CreatePriceDetails(GameGoldCurrency, 150)) }
        };
        collection.InsertOne(item2);
    }

    protected override void DownExecute(IMongoDatabase database)
    {
        var collection = database.GetCollection<BsonDocument>("ItemPrice");
        collection.DeleteOne(x => x[IdField] == ItemPriceId1);
        
        collection.DeleteOne(x => x[IdField] == ItemPriceId2);
    }

    private static IEnumerable<BsonDocument> CreatePriceDetails(string currency, decimal price)
    {
        var slots = new List<BsonDocument>
        {
            new()
            {
                { "Currency", currency },
                { "Price", price }
            }
        };

        return slots;
    }
}