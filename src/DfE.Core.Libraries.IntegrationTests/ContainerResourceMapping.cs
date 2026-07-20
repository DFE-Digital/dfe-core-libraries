namespace DfE.Core.Libraries.IntegrationTests.Abstractions;

public sealed class ContainerResourceMapping
{
    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public bool ReadOnly { get; set; } = true;
    public bool Executable { get; set; } = false;
}
