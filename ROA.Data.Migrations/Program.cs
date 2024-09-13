using System.Reflection;
using CommandLine;
using Microsoft.Extensions.Configuration;
using MongoDBMigrations;

public class Options
{
    [Option('v', "version", Required = false, HelpText = "Set the target version to migrate")]
    public string Version { get; set; }

    [Option('c', "connectionString", Required = false, HelpText = "Set Server ConnectionString")]
    public string ConnectionString { get; set; }

    [Option('d', "database", Required = false, HelpText = "Set the Database name")]
    public string Database { get; set; }

}

public static class Program
{
    public static void Main(string[] args)
    {
        var config = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json", optional: false)
                   .Build();

        var connectionSettings = config.GetSection("MongoConnection");
        var connectionString = connectionSettings.GetValue<string>("ConnectionString");
        var database = connectionSettings.GetValue<string>("DatabaseName");
        var version = config.GetValue<string>("Version");
        var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (s, e) =>
        {
            Console.WriteLine("Canceling...");
            cts.Cancel();
            e.Cancel = true;
        };

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
            {
                if (!string.IsNullOrEmpty(o.ConnectionString))
                {
                    connectionString = o.ConnectionString;
                }
                if (!string.IsNullOrEmpty(o.Database))
                {
                    database = o.Database;
                }
                if (!string.IsNullOrEmpty(o.Version))
                { 
                    version = o.Version;
                }
            });

        var runner = new MigrationEngine()
                .UseDatabase(connectionString, database)
                .UseAssembly(Assembly.GetExecutingAssembly())
                .UseSchemeValidation(false)
                .UseCancelationToken(cts.Token);

        Console.WriteLine("Update started...");

        if (!string.IsNullOrEmpty(version))
        {
           
            runner.Run(version);
        }
        else
        {
            runner.Run();
        }
       
        Console.WriteLine("Update completed");
    }
}