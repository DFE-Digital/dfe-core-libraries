using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;

public sealed record ContainerBuilderHandlerRegistration<TBuilder> where TBuilder : class
{
    public ContainerBuilderHandlerRegistration(string key, IContainerBuilderHandler<TBuilder> handler)
    {
        Key = key;
        Handler = handler;
    }

    public string Key { get; }
    public IContainerBuilderHandler<TBuilder> Handler { get; }
}
