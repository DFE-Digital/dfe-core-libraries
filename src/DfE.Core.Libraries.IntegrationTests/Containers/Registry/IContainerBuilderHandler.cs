namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

public interface IContainerBuilderHandler<TBuilder>
    where TBuilder : class
{
    ValueTask<TBuilder> ApplyAsync(
        TBuilder builder,
        CancellationToken cancellationToken);
}
