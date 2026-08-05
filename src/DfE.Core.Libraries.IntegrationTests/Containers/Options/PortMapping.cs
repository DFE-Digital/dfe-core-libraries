namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options;

public sealed class PortMapping
{
    public int? PublicPort { get; set; }
    public int ContainerPort { get; set; }
}
