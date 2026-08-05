using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers;

public sealed class ContainerRegistry : IContainerRegistry, IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, Lazy<Task<NetworkRegistration>>> _networks =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly ConcurrentDictionary<string, IContainer> _containers =
        new(StringComparer.OrdinalIgnoreCase);

    private bool _disposed;

    public async Task<INetwork> GetOrCreateNetworkAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException(
                "Network key cannot be null or whitespace.",
                nameof(key));
        }

        Lazy<Task<NetworkRegistration>> registration =
            _networks.GetOrAdd(
                key,
                CreateNetwork);

        return (await registration.Value).Network;
    }

    public Task RegisterContainerAsync(
        string name,
        IContainer container)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Container name cannot be null or whitespace.",
                nameof(name));
        }

        if (container == null)
        {
            throw new ArgumentException("Container cannot be null");
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
            throw new ArgumentException(
                "Container name cannot be null or whitespace.",
                nameof(name));
        }

        return _containers.TryGetValue(name, out container);
    }

    private static Lazy<Task<NetworkRegistration>> CreateNetwork(
        string key)
    {
        return new Lazy<Task<NetworkRegistration>>(
            async () =>
            {
                string networkName = CreateDockerNetworkName(key);

                INetwork network = new NetworkBuilder()
                    .WithName(networkName)
                    .Build();

                await network.CreateAsync();

                return new NetworkRegistration(
                    key,
                    networkName,
                    network);
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

        foreach (Lazy<Task<NetworkRegistration>> registration in _networks.Values.Reverse())
        {
            try
            {
                if (registration.IsValueCreated)
                {
                    NetworkRegistration networkRegistration =
                        await registration.Value;

                    await networkRegistration.Network.DisposeAsync();
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
    private sealed record NetworkRegistration
    {
        public NetworkRegistration(string Key, string DockerNetworkName, INetwork Network)
        {
            this.Key = Key;
            this.DockerNetworkName = DockerNetworkName;
            this.Network = Network;
        }

        public string Key { get; }
        public string DockerNetworkName { get; }
        public INetwork Network { get; }
    };

    private static string CreateDockerNetworkName(string key)
    {
        string sanitizedKey = Regex.Replace(
            key.ToLowerInvariant(),
            "[^a-z0-9_.-]",
            "-");

        sanitizedKey = Regex.Replace(sanitizedKey, "-+", "-");

        sanitizedKey = sanitizedKey.Trim('-');

        return $"{sanitizedKey}-{Guid.NewGuid():N}";
    }
}
