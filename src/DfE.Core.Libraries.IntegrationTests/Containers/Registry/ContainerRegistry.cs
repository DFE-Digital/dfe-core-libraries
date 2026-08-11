using System.Collections.Concurrent;
using DotNet.Testcontainers.Containers;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

internal sealed class ContainerRegistry : IContainerRegistry, IAsyncDisposable
{
    private readonly IContainerFactoryRegistry _factoryRegistry;

    private readonly ConcurrentDictionary<
        string,
        Lazy<Task<IContainer>>> _containers =
            new(StringComparer.OrdinalIgnoreCase);

    private bool _disposed;

    public ContainerRegistry(
        IContainerFactoryRegistry factoryRegistry)
    {
        _factoryRegistry = factoryRegistry;
    }

    public async Task<IContainer> GetOrCreateContainerAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Container key cannot be null or whitespace");
        }

        Lazy<Task<IContainer>> lazyContainer =
            _containers.GetOrAdd(
                key,
                (key) => new Lazy<Task<IContainer>>(
                    async () =>
                    {
                        IContainerFactory factory =
                            _factoryRegistry.GetFactory(key);

                        IContainer container =
                            await factory.CreateAsync(
                                key,
                                cancellationToken);

                        await container.StartAsync(
                            cancellationToken);

                        return container;
                    },
                    LazyThreadSafetyMode.ExecutionAndPublication));

        return await lazyContainer.Value;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        foreach (Lazy<Task<IContainer>> registration in _containers.Values.Reverse())
        {
            if (registration.IsValueCreated)
            {
                await (await registration.Value)
                    .DisposeAsync();
            }
        }

        _containers.Clear();
    }
}
