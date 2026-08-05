namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options;

public sealed class ContainerNetworkAttachmentOptions
{
    public IEnumerable<NetworkAttachment> Networks { get; set; } = [];
}

public sealed class NetworkAttachment
{
    public string? Name { get; set; }
    public IList<string> Aliases { get; set; } = [];
}
