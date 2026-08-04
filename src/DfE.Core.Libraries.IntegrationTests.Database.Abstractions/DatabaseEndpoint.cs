namespace DfE.Core.Libraries.IntegrationTests.Database.Abstractions;

public record DatabaseEndpoint
{
    public DatabaseEndpoint(string host, ushort port)
    {
        Host = string.IsNullOrWhiteSpace(host) ? string.Empty : host;
        Port = port;
    }

    public string Host { get; }
    public ushort Port { get; }
}
