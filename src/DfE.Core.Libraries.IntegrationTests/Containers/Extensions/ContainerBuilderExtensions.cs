using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Container;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;

public static class ContainerBuilderExtensions
{
    // Note: public as shared with Postgres project could be split 
    public static TBuilder WithContainerOptions<TBuilder, TContainer, TConfiguration>(this TBuilder builder, ContainerOptions options)
            where TContainer : IContainer
            where TConfiguration : IContainerConfiguration
            where TBuilder : ContainerBuilder<TBuilder, TContainer, TConfiguration>
    {
        builder =
            builder
                .WithImage(options.Image)
                .WithExposedPorts<TBuilder, TContainer, TConfiguration>(options.PortMappings ?? [])
                .WithStartupCommands<TBuilder, TContainer, TConfiguration>(options.StartupArguments ?? [])
                .WithMountedResources<TBuilder, TContainer, TConfiguration>(options.CopyResourcesIntoContainerBeforeInit ?? []);

        if (options.Labels.Any())
        {
            builder =
                builder.WithLabel(
                    options.Labels.ToDictionary(x => x.Key?.Trim(), x => x.Value?.Trim()));
        }

        if (options.Env.Any())
        {
            builder =
                builder.WithEnvironment(
                    options.Env.ToDictionary((x) => x.Key?.Trim(), x => x.Value?.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(options.ContainerName))
        {
            builder = builder.WithName(options.ContainerName);
        }

        return builder;
    }

    // Note: public as shared with Postgres project could be split 
    public static async Task<TBuilder> WithContainerNetworksAsync<TBuilder, TContainer, TConfiguration>(
        this TBuilder builder,
        IEnumerable<ContainerNetworkAttachment>? networks,
        IContainerNetworkRegistry registry)
            where TContainer : IContainer
            where TConfiguration : IContainerConfiguration
            where TBuilder : ContainerBuilder<TBuilder, TContainer, TConfiguration>
    {
        if (networks == null)
        {
            return builder;
        }

        foreach (ContainerNetworkAttachment networkOption in networks)
        {

            INetwork network =
                await registry.GetOrCreateNetworkAsync(
                    $"{networkOption.Key}");

            builder = builder.WithNetwork(network);

            foreach (string alias in networkOption.Aliases)
            {
                builder = builder.WithNetworkAliases(alias);
            }
        }

        return builder;
    }

    private static TBuilder WithExposedPorts<
        TBuilder,
        TContainer,
        TConfiguration>(
        this TBuilder builder,
        IEnumerable<PortMapping> portMappings)
            where TContainer : IContainer
            where TConfiguration : IContainerConfiguration
            where TBuilder : ContainerBuilder<
                    TBuilder,
                    TContainer,
                    TConfiguration>
    {
        foreach (PortMapping portMapping in portMappings)
        {
            builder = builder.WithExposedPort(portMapping.ContainerPort);

            builder = portMapping.PublicPort.HasValue
                ? builder.WithPortBinding(
                    portMapping.PublicPort.Value,
                    portMapping.ContainerPort)
                : builder.WithPortBinding(
                    portMapping.ContainerPort,
                    assignRandomHostPort: true);
        }

        return builder;
    }


    private static TBuilder WithStartupCommands<
        TBuilder,
        TContainer,
        TConfiguration>(
        this TBuilder builder,
        IEnumerable<StartupArgument>? args)
            where TContainer : IContainer
            where TConfiguration : IContainerConfiguration
            where TBuilder : ContainerBuilder<
                    TBuilder,
                    TContainer,
                    TConfiguration>
    {
        if (args == null || !args.Any())
        {
            return builder;
        }

        string?[] flattenedArgs = args?
            .SelectMany(
                kv => kv.Value.SelectMany(
                    value => new[]
                    {
                    kv.Key,
                    value.Trim()
                    }))
            .ToArray() ?? [];

        return builder.WithCommand(flattenedArgs);
    }

    private static TBuilder WithMountedResources<
        TBuilder,
        TContainer,
        TConfiguration>(
        this TBuilder builder,
        IEnumerable<ContainerResourceMapping>? resources)
            where TContainer : IContainer
            where TConfiguration : IContainerConfiguration
            where TBuilder : ContainerBuilder<
                TBuilder,
                TContainer,
                TConfiguration>
    {
        foreach (ContainerResourceMapping resource in resources ?? [])
        {
            builder = builder.WithResourceMapping(
                source: resource.Source,
                target: resource.Destination,
                fileMode: GetFileMode(resource));
        }

        return builder;

        static UnixFileModes GetFileMode(ContainerResourceMapping resource)
        {
            UnixFileModes mode =
                UnixFileModes.UserRead |
                UnixFileModes.GroupRead |
                UnixFileModes.OtherRead;

            if (!resource.ReadOnly)
            {
                mode |= UnixFileModes.UserWrite;
            }

            if (resource.Executable)
            {
                mode |= UnixFileModes.UserExecute;
            }

            return mode;
        }
    }
}
