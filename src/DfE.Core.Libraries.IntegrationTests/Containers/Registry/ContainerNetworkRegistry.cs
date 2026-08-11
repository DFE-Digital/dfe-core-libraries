using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

internal sealed class ContainerNetworkRegistry : IContainerNetworkRegistry
{
    private readonly ConcurrentDictionary<string, Lazy<Task<NetworkRegistration>>> _networks = new(StringComparer.OrdinalIgnoreCase);

    private bool _disposed;
    public async Task<INetwork> GetOrCreateNetworkAsync(string key)
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

        foreach (Lazy<Task<NetworkRegistration>> registration in _networks.Values.Reverse())
        {
            if (registration.IsValueCreated)
            {
                await (await registration.Value)
                    .Network
                    .DisposeAsync();
            }
        }

        _networks.Clear();
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
}
