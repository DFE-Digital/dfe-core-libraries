namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry.BuilderHandler;

public interface IConfigureContainerBuilderHandler<TBuilder>
    where TBuilder : class
{
    ValueTask<TBuilder> HandleAsync(
        TBuilder builder,
        CancellationToken cancellationToken);
}
