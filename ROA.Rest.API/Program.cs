using System.Text.Json.Serialization;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ROA.Infrastructure.Data;
using ROA.Infrastructure.Data.Mongo;
using ROA.Infrastructure.Trace.Extensions;
using ROA.Rest.API.Data.Mapping;
using ROA.Rest.API.Data.Repositories;
using ROA.Rest.API.Mappers;
using ROA.Rest.API.Settings;
using Serilog;

namespace ROA.Rest.API;

public class Program
{
    public static void Main(string[] args)
    {
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

        ConfigureLogger();
        
        ConfigureTracing(builder);

        builder.Host.UseSerilog();

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        ConfigureSettings(builder);
        ConfigureRepositories(builder);

        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddHttpContextAccessor();
        return builder;
    }

    private static void ConfigureLogger()
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
    }

    private static void ConfigureTracing(WebApplicationBuilder builder)
    {
        var settings = builder.Configuration.GetSection("Tracing").Get<TracingSettings>();

        if (settings == null)
        {
            return;
        }

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracerBuilder =>
            {
                tracerBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(settings.ServiceName))
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddMongoDbInstrumentation()
                    .AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(settings.Url);
                        o.Protocol = settings.Protocol == TracingProtocol.Grpc
                            ? OtlpExportProtocol.Grpc
                            : OtlpExportProtocol.HttpProtobuf;
                    });
            });
    }

    private static void ConfigureRepositories(IHostApplicationBuilder builder)
    {
        MapperFactory.Configure(builder.Services);
        
        DataContextMap.CreateMaps();

        builder.Services.AddSingleton<IDataContext, DataContext>();

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