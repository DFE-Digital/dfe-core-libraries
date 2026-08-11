namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Container;

public sealed class PortMapping
{
    public int? PublicPort { get; set; }
    public int ContainerPort { get; set; }
}
