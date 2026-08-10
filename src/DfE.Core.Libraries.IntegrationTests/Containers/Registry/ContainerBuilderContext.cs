using DotNet.Testcontainers.Containers;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

// TestContainers does not have a shared abstraction across builders
public sealed class ContainerBuilderContext<TBuilder> where TBuilder : class
{
    private readonly Func<TBuilder, IContainer> _build;

    public ContainerBuilderContext(
        TBuilder builder,
        Func<TBuilder, IContainer> build)
    {
        Builder = builder;
        _build = build;
    }

    public TBuilder Builder { get; private set; }

    public IContainer Build()
    {
        return _build(Builder);
    }

    public void ReplaceBuilder(TBuilder builder)
    {
        Builder = builder;
    }
}
