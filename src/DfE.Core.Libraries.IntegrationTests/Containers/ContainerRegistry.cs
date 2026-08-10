using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers;

internal sealed class ContainerRegistry : IContainerRegistry, IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, ContainerRegistration> _registrations =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly ConcurrentDictionary<string, Lazy<Task<IContainer>>> _containers =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly ConcurrentDictionary<string, Lazy<Task<NetworkRegistration>>> _networks =
        new(StringComparer.OrdinalIgnoreCase);

    private bool _disposed;

    public ContainerRegistry(IEnumerable<ContainerRegistration>? containerRegistrations)
    {
        containerRegistrations?
            .ToList()
            .ForEach((containerRegistrations)
                => _registrations.GetOrAdd(containerRegistrations.Key, containerRegistrations));
    }

    public void Register(
        string key,
        Func<IContainerRegistry, CancellationToken, Task<IContainer>> create)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Container key cannot be null or whitespace.", nameof(key));
        }

        if (!_registrations.TryAdd(
                key,
                new ContainerRegistration(
                    key,
                    create)))
        {
            throw new InvalidOperationException(
                $"Container '{key}' is already registered.");
        }
    }

    public async Task<IContainer> GetOrCreateContainerAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Container key cannot be null or whitespace.", nameof(key));
        }

        if (!_registrations.TryGetValue(key, out ContainerRegistration? registration))
        {
            throw new InvalidOperationException($"Container: {key} has not been registered.");
        }

        Lazy<Task<IContainer>> lazyContainer =
            _containers.GetOrAdd(
                key,
                _ => new Lazy<Task<IContainer>>(
                    async () =>
                    {
                        IContainer container =
                            await registration.Create(
                                this,
                                cancellationToken);

                        await container.StartAsync(
                            cancellationToken);

                        return container;
                    },
                    LazyThreadSafetyMode.ExecutionAndPublication));

        return await lazyContainer.Value;
    }

    public async Task<INetwork> GetOrCreateNetworkAsync(
        string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Container key cannot be null or whitespace.", nameof(key));
        }

        Lazy<Task<NetworkRegistration>> registration =
            _networks.GetOrAdd(
                key,
                CreateNetwork);

        return (await registration.Value).Network;
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

        foreach (Lazy<Task<NetworkRegistration>> registration in _networks.Values.Reverse())
        {
            if (registration.IsValueCreated)
            {
                await (await registration.Value)
                    .Network
                    .DisposeAsync();
            }
        }

        _containers.Clear();
        _registrations.Clear();
        _networks.Clear();
    }

    // Existing NetworkRegistration and CreateNetwork implementation.

    private sealed record NetworkRegistration
    {
        public NetworkRegistration(string key,
        string dockerNetworkName,
        INetwork network)
        {
            Key = key;
            DockerNetworkName = dockerNetworkName;
            Network = network;
        }

        public string Key { get; }
        public string DockerNetworkName { get; }
        public INetwork Network { get; }
    }

    private static Lazy<Task<NetworkRegistration>> CreateNetwork(string key)
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

    private static string CreateDockerNetworkName(string key)
    {
        string sanitizedKey =
            Regex.Replace(
                key.ToLowerInvariant(),
                "[^a-z0-9_.-]",
                "-");

        sanitizedKey =
            Regex.Replace(
                sanitizedKey,
                "-+",
                "-");

        sanitizedKey =
            sanitizedKey.Trim('-');

        return $"{sanitizedKey}-{Guid.NewGuid():N}";
    }
}
