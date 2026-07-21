using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions;

public static class ContainerBuilderExtensions
{
    public static TBuilder WithExposedPorts<
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


    public static TBuilder WithStartupCommands<
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

    public static TBuilder WithMountedResources<
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
