namespace DfE.Core.Libraries.IntegrationTests.Abstractions;

public sealed class PortMapping
{
    public int? PublicPort { get; set; }
    public int ContainerPort { get; set; }
}
