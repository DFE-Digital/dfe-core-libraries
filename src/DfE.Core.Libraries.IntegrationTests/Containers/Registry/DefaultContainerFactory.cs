using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Container;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry.BuilderHandler;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Options;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

public sealed class DefaultContainerFactory : IContainerFactory
{
    private readonly IContainerNetworkRegistry _networkRegistry;
    private readonly IOptionsMonitor<ContainerOptions> _optionsMonitor;
    private readonly IReadOnlyDictionary<
            string,
            Func<IReadOnlyCollection<IConfigureContainerBuilderHandler<ContainerBuilder>>>> _handlersRegistry;

    public DefaultContainerFactory(
        IOptionsMonitor<ContainerOptions> optionsMonitor,
        IContainerNetworkRegistry networkRegistry,
        Dictionary<string, Func<IReadOnlyCollection<IConfigureContainerBuilderHandler<ContainerBuilder>>>> handlerRegistry)
    {
        _optionsMonitor = optionsMonitor;
        _networkRegistry = networkRegistry;
        _handlersRegistry = handlerRegistry ?? [];
    }

    public async Task<IContainer> CreateAsync(string key, CancellationToken cancellationToken)
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

        if (_handlersRegistry.TryGetValue(key, out Func<IReadOnlyCollection<IConfigureContainerBuilderHandler<ContainerBuilder>>>? handlers))
        {
            foreach (IConfigureContainerBuilderHandler<ContainerBuilder> handler in handlers.Invoke())
            {
                builder = await handler.HandleAsync(builder, cancellationToken);
            }
        }

        return builder.Build();
    }
}
