namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options;


public sealed record StartupArgument
{
    public string? Key { get; set; }

    public string[]? Value { get; set; }
}
