using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBMigrations;

namespace ROA.Data.Migrations.Versions;

public class BaseMigration : IMigration
{
    public MongoDBMigrations.Version Version => new(1, 0, 0);

    public string Name => "Base migration";

    public void Up(IMongoDatabase database)
    {
        Console.WriteLine("Updating collections");

        Console.WriteLine("Migration completed");
    }

    public void Down(IMongoDatabase database)
    {
        Console.WriteLine("Updating collections");

        Console.WriteLine("Migration completed");
    }
}