using System.Text.Json.Serialization;
using ROA.Data;
using ROA.Data.Contract;
using ROA.Data.Contract.Repositories;
using ROA.Data.Repositories;
using ROA.Rest.API.Mappers;
using Serilog;

namespace ROA.Rest.API;

public class Program
{
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                                        ?? "Production"}.json", true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        var builder = CreateBuilder(args);

        var app = builder.Build();

        app.UseSerilogRequestLogging();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static WebApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog();

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        ConfigureSettings(builder);
        ConfigureServices(builder);

        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddHttpContextAccessor();
        return builder;
    }

    private static void ConfigureServices(IHostApplicationBuilder builder)
    {
        MapperFactory.Configure(builder.Services);
        ConfigureRepositories(builder);
    }

    private static void ConfigureRepositories(IHostApplicationBuilder builder)
    {
        DataContext.CreateMaps();

        builder.Services.AddSingleton<IDataContext, DataContext>();

        builder.Services.AddSingleton<IInventoryRepository, InventoryRepository>();
        builder.Services.AddSingleton<IPlayerRepository, PlayerRepository>();
        builder.Services.AddSingleton<IPaymentRepository, PaymentRepository>();
        builder.Services.AddSingleton<IItemPriceRepository, ItemPriceRepository>();

        builder.Services.AddScoped<IDataContextManager, DataContextManager>();
    }

    private static void ConfigureSettings(WebApplicationBuilder builder)
    {
        builder.Services.Configure<ConnectionDatabaseSettings>(
            builder.Configuration.GetSection("MongoConnection"));
    }
}