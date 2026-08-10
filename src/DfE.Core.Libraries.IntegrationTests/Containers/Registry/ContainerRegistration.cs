using DotNet.Testcontainers.Containers;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

public sealed class ContainerRegistration<TBuilder>
    : IContainerRegistration where TBuilder : class
{
    private readonly Func<
        IContainerRegistry,
        CancellationToken,
        Task<ContainerBuilderContext<TBuilder>>> _createBuilder;

    private readonly IReadOnlyCollection<IContainerBuilderHandler<TBuilder>> _handlers;

    public ContainerRegistration(
        string key,
        Func<
            IContainerRegistry,
            CancellationToken,
            Task<ContainerBuilderContext<TBuilder>>> createBuilder,
        IEnumerable<IContainerBuilderHandler<TBuilder>> handlers)
    {
        Key = key;
        _createBuilder = createBuilder;
        _handlers = handlers?.ToList() ?? [];
    }

    public string Key { get; }

    public async Task<IContainer> CreateAsync(
        IContainerRegistry registry,
        CancellationToken cancellationToken)
    {
        ContainerBuilderContext<TBuilder> definition =
            await _createBuilder(
                registry,
                cancellationToken);

        foreach (IContainerBuilderHandler<TBuilder> handler in _handlers)
        {
            TBuilder builder = await handler.ApplyAsync(definition.Builder, cancellationToken);
            definition.ReplaceBuilder(builder);
        }

        return definition.Build();
    }
}
