using OpenTelemetry.Trace;

namespace ROA.Rest.API.Extensions;

public static class TracerProviderExtension
{
    public static TracerProviderBuilder AddMongoDbInstrumentation(this TracerProviderBuilder builder)
    {
        return builder.AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources");
    }
}