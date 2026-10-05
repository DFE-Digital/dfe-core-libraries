namespace DfE.Core.Libraries.IntegrationTests.Containers.Registry.Builder;

public interface IConfigureContainerBuilderHandler<TBuilder>
    where TBuilder : class
{
    ValueTask<TBuilder> HandleAsync(
        TBuilder builder,
        CancellationToken cancellationToken);
}
