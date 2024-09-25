using MongoDB.Driver;
using MongoDBMigrations;

namespace ROA.Data.Migrations.Versions;

public abstract class Migration : IMigration
{
    public required MongoDBMigrations.Version Version {get; init;}

    public required string Name {get; init;}


    public void Up(IMongoDatabase database)
    {
        Console.WriteLine($"UP: Updating collections {Version.ToString()}");
        UpExecute(database);
        Console.WriteLine($"UP: Migration completed {Version.ToString()}");
    }

    public void Down(IMongoDatabase database)
    {
        Console.WriteLine($"DOWN: Updating collections {Version.ToString()}");
        DownExecute(database);
        Console.WriteLine($"DOWN: Migration completed {Version.ToString()}");
    }
    
    protected abstract void UpExecute(IMongoDatabase database);
    protected abstract void DownExecute(IMongoDatabase database);
}