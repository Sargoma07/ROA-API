namespace ROA.Rest.API;

public class TracingSettings
{
    public string ServiceName { get; set; } = null!;
    public string Url { get; set; } = null!;
    public TracingProtocol Protocol { get; set; }
    public string Provider { get; set; } = null!;
}

public enum TracingProtocol
{
    Grpc,
    Protobuf
}