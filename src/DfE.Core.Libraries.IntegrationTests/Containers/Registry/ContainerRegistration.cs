using DotNet.Testcontainers.Containers;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

public sealed class ContainerRegistration<TBuilder>
    : IContainerRegistration where TBuilder : class
{
    private readonly Func<
        IContainerRegistry,
        CancellationToken,
        Task<TBuilder>> _createBuilder;

    private readonly Func<
        TBuilder,
        IContainer> _build;

    private readonly IReadOnlyCollection<IContainerBuilderHandler<TBuilder>> _handlers;

    public ContainerRegistration(
        string key,
        Func<
            IContainerRegistry,
            CancellationToken,
            Task<TBuilder>> createBuilder,
        Func<TBuilder, IContainer> build,
        IEnumerable<IContainerBuilderHandler<TBuilder>> handlers)
    {
        Key = key;
        _createBuilder = createBuilder;
        _build = build;
        _handlers = handlers?.ToList() ?? [];
    }

    public string Key { get; }

    public async Task<IContainer> CreateAsync(
        IContainerRegistry registry,
        CancellationToken cancellationToken)
    {
        TBuilder builder =
            await _createBuilder(
                registry,
                cancellationToken);

        foreach (IContainerBuilderHandler<TBuilder> handler in _handlers)
        {
            builder = await handler.ApplyAsync(
                builder,
                cancellationToken);
        }

        return _build(builder);
    }
}
