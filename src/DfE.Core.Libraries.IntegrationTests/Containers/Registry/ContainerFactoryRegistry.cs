namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

internal sealed class ContainerFactoryRegistry
    : IContainerFactoryRegistry
{
    private readonly IContainerFactory _defaultFactory;

    private readonly IReadOnlyDictionary<string, IContainerFactory> _factories;

    public ContainerFactoryRegistry(
        DefaultContainerFactory defaultFactory,
        IEnumerable<KeyValuePair<string, IContainerFactory>> factories)
    {
        _defaultFactory = defaultFactory;

        _factories =
            factories.ToDictionary(
                x => x.Key,
                x => x.Value,
                StringComparer.OrdinalIgnoreCase);
    }

    public IContainerFactory GetFactory(string key)
    {
        return _factories.TryGetValue(
            key,
            out IContainerFactory? factory)
            ? factory
            : _defaultFactory;
    }
}
