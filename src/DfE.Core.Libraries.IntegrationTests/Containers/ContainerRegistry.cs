using System.Collections.Concurrent;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers;

public sealed class ContainerRegistry : IContainerRegistry, IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, Lazy<Task<INetwork>>> _networks = new(StringComparer.OrdinalIgnoreCase);

    private readonly ConcurrentDictionary<string, IContainer> _containers = new(StringComparer.OrdinalIgnoreCase);

    private bool _disposed;

    public async Task<INetwork> GetOrCreateNetworkAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Network name cannot be null or whitespace.", nameof(name));
        }

        Lazy<Task<INetwork>> lazyNetwork = _networks.GetOrAdd(
            name,
            CreateNetwork);

        return await lazyNetwork.Value;
    }

    public Task RegisterContainerAsync(
        string name,
        IContainer container)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Container name cannot be null or whitespace.", nameof(name));
        }

        if (container is null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        if (!_containers.TryAdd(name, container))
        {
            throw new InvalidOperationException(
                $"A container named '{name}' has already been registered.");
        }

        return Task.CompletedTask;
    }

    public bool TryGetContainer(
        string name,
        out IContainer? container)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Container name cannot be null or whitespace.", nameof(name));
        }

        return _containers.TryGetValue(name, out container);
    }


    private static Lazy<Task<INetwork>> CreateNetwork(string networkName)
    {
        return new Lazy<Task<INetwork>>(
            async () =>
            {
                INetwork network = new NetworkBuilder()
                    .WithName(networkName)
                    .Build();

                await network.CreateAsync();

                return network;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        List<Exception> exceptions = [];

        //
        // Dispose containers first.
        //
        foreach (IContainer container in _containers.Values.Reverse())
        {
            try
            {
                await container.DisposeAsync();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        //
        // Dispose networks after containers.
        //
        foreach (Lazy<Task<INetwork>> lazyNetwork in _networks.Values.Reverse())
        {
            try
            {
                if (lazyNetwork.IsValueCreated)
                {
                    INetwork network = await lazyNetwork.Value;
                    await network.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        _containers.Clear();
        _networks.Clear();

        if (exceptions.Count > 0)
        {
            throw new AggregateException(
                "One or more errors occurred while disposing container resources.",
                exceptions);
        }
    }
}
