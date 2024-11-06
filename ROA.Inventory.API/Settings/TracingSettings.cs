namespace ROA.Inventory.API.Settings;

public record TracingSettings
{
    public required string ServiceName { get; init; }
    public required string Url { get; init; }
    public TracingProtocol Protocol { get; init; }
    public required string Provider { get; init; }
}

public enum TracingProtocol
{
    Grpc,
    HttpProtobuf
}