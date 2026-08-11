using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Container;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Options;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

public sealed class DefaultContainerFactory : IContainerFactory
{
    private readonly IContainerNetworkRegistry _networkRegistry;
    private readonly IOptionsMonitor<ContainerOptions> _optionsMonitor;
    private readonly IReadOnlyCollection<IContainerBuilderHandler<ContainerBuilder>> _handlers; // TODO selecting handlers that only apply to a given container registration - currently every container gets every handler

    public DefaultContainerFactory(
        IOptionsMonitor<ContainerOptions> optionsMonitor,
        IContainerNetworkRegistry networkRegistry,
        IEnumerable<IContainerBuilderHandler<ContainerBuilder>> handlers)
    {
        _optionsMonitor = optionsMonitor;
        _networkRegistry = networkRegistry;
        _handlers = handlers?.ToArray() ?? [];
    }

    public async Task<IContainer> CreateAsync(
        string key,
        CancellationToken cancellationToken)
    {
        ContainerOptions options = _optionsMonitor.Get(key) ?? throw new ArgumentException($"ContainerOptions for {key} not registered");

        ContainerBuilder builder =
            new(options.Image);

        builder =
            builder.WithContainerOptions<
                ContainerBuilder,
                IContainer,
                IContainerConfiguration>(options);

        builder =
            await builder.WithContainerNetworksAsync<
                ContainerBuilder,
                IContainer,
                IContainerConfiguration>(options.Networks, _networkRegistry);

        foreach (IContainerBuilderHandler<ContainerBuilder> handler in _handlers)
        {
            builder =
                await handler.ApplyAsync(
                    builder,
                    cancellationToken);
        }

        return builder.Build();
    }
}
