using System.Text;
using System.Text.Json.Serialization;
using Confluent.Kafka.Extensions.OpenTelemetry;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ROA.Infrastructure.Data;
using ROA.Infrastructure.Data.Mongo;
using ROA.Infrastructure.EventBus.Kafka;
using ROA.Infrastructure.Trace.Extensions;
using ROA.Payment.API.Data;
using ROA.Payment.API.Data.Mapping;
using ROA.Payment.API.Data.Repositories;
using ROA.Payment.API.EventBus;
using ROA.Payment.API.Filters;
using ROA.Payment.API.Mappers;
using ROA.Payment.API.Settings;
using Serilog;

namespace ROA.Payment.API;

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

        app.UseAuthentication();
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

        ConfigureAuthentication(builder);
        
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        ConfigureSettings(builder);
        ConfigureRepositories(builder);
        ConfigureEventBus(builder);

        builder.Services
            .AddControllers(options =>
            {
                options.Filters.Add<UserActionFilter>();
            })
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
                    .AddConfluentKafkaInstrumentation()
                    .AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(settings.Url);
                        o.Protocol = settings.Protocol == TracingProtocol.Grpc
                            ? OtlpExportProtocol.Grpc
                            : OtlpExportProtocol.HttpProtobuf;
                    });
            });
    }
    
    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        var settings = builder.Configuration.GetSection("Auth").Get<AuthSettings>();

        if (settings is null)
        {
            throw new InvalidOperationException("Auth settings not found");
        }

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = settings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = settings.Audience,
                        ValidateLifetime = true,
                        IssuerSigningKey = new  SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret)),
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
    }

    private static void ConfigureRepositories(IHostApplicationBuilder builder)
    {
        MapperFactory.Configure(builder.Services);
        
        DataContextMap.CreateMaps();
        
        builder.Services.AddScoped<IAccountContext, AccountContext>();

        builder.Services.AddSingleton<IDataContext, DataContext>();

        builder.Services.AddSingleton<IPaymentRepository, PaymentRepository>();
        builder.Services.AddSingleton<IItemPriceRepository, ItemPriceRepository>();
        builder.Services.AddSingleton<IAccountRepository, AccountRepository>();

        builder.Services.AddScoped<IDataContextManager, DataContextManager>();
    }

    private static void ConfigureSettings(WebApplicationBuilder builder)
    {
        builder.Services.Configure<ConnectionDatabaseSettings>(
            builder.Configuration.GetSection("MongoConnection"));
        
        builder.Services.Configure<KafkaSettings>(
            builder.Configuration.GetSection("Kafka"));
        
        builder.Services.Configure<TopicSettings>(
            builder.Configuration.GetSection("Kafka:Topics"));
    }
    
    private static void ConfigureEventBus(WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<UserCreatedConsumerService>();
        builder.Services.AddSingleton<UserCreatedConsumer>();
        builder.Services.AddScoped<UserCreatedConsumeStrategy>();
    }
}