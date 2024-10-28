using MongoDB.Driver;
using Version = MongoDBMigrations.Version;

namespace ROA.Data.Migrations.Versions;

public class BaseMigration : Migration
{
    public BaseMigration()
    {
        Version = new Version(1, 0, 0);
        Name = "Base migration";
    }

    protected override void UpExecute(IMongoDatabase database)
    {
    }

    protected override void DownExecute(IMongoDatabase database)
    {
    }
}