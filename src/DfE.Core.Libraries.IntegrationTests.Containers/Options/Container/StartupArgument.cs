namespace DfE.Core.Libraries.IntegrationTests.Containers.Options.Container;


public sealed record StartupArgument
{
    public string? Key { get; set; }

    public string[]? Value { get; set; }
}
