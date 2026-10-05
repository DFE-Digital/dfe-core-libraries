namespace DfE.Core.Libraries.IntegrationTests.Containers.Options.Container;

public sealed class ContainerNetworkAttachment
{
    public string Key { get; set; } = string.Empty;
    public IList<string> Aliases { get; set; } = [];
}

