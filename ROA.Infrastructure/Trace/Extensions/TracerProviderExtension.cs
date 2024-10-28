using OpenTelemetry.Trace;

namespace ROA.Infrastructure.Trace.Extensions;

public static class TracerProviderExtension
{
    public static TracerProviderBuilder AddMongoDbInstrumentation(this TracerProviderBuilder builder)
    {
        return builder.AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources");
    }
}