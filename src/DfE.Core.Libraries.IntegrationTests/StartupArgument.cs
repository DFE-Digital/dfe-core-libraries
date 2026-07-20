namespace DfE.Core.Libraries.IntegrationTests.Abstractions;


public sealed record StartupArgument
{
    public string? Key { get; set; }

    public string[]? Value { get; set; }
}
