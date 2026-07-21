namespace DfE.Core.Libraries.IntegrationTests.Abstractions;

public sealed class ContainerOptions
{

    public string Registry { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;

#nullable disable
    public string ImageName { get; set; }
#nullable enable
    public string? ImageTag { get; set; } = "latest";
    public string? Digest { get; set; }

    public string Image
    {
        get
        {
            string imageReference;

            if (!string.IsNullOrWhiteSpace(Registry))
            {
                imageReference = string.IsNullOrWhiteSpace(Owner)
                        ? $"{Registry}/{ImageName}"
                        : $"{Registry}/{Owner}/{ImageName}";
            }
            else
            {
                imageReference = !string.IsNullOrWhiteSpace(Owner) ? $"{Owner}/{ImageName}" : ImageName;
            }

            if (!string.IsNullOrWhiteSpace(ImageTag))
            {
                imageReference += $":{ImageTag}";
            }

            if (!string.IsNullOrWhiteSpace(Digest))
            {
                imageReference += $"@{Digest}";
            }

            return imageReference;
        }
    }
    public string HostName { get; set; } = "localhost";
    public IEnumerable<PortMapping>? PortMappings { get; set; }
    public IEnumerable<StartupArgument>? StartupArguments { get; set; } = [];
    public IEnumerable<ContainerResourceMapping>? CopyResourcesIntoContainerBeforeInit { get; set; } = [];
}
