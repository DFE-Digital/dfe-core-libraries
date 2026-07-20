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
        ContainerOptions options,
        ushort containerPort)
    where TContainer : IContainer
    where TConfiguration : IContainerConfiguration
    where TBuilder : ContainerBuilder<
            TBuilder,
            TContainer,
            TConfiguration>
    {
        builder = builder.WithExposedPort(containerPort);

        return options.PublicPort.HasValue
            ? builder.WithPortBinding(
                options.PublicPort.Value,
                containerPort)

            : builder.WithPortBinding(
                containerPort,
                assignRandomHostPort: true);
    }


    public static TBuilder WithStartupCommands<
        TBuilder,
        TContainer,
        TConfiguration>(
        this TBuilder builder,
        ContainerOptions options)
    where TContainer : IContainer
    where TConfiguration : IContainerConfiguration
    where TBuilder : ContainerBuilder<
            TBuilder,
            TContainer,
            TConfiguration>
    {
        string?[] args = options.StartupArguments
            .SelectMany(
                kv => kv.Value.SelectMany(
                    value => new[]
                    {
                    kv.Key,
                    value.Trim()
                    }))
            .ToArray();

        return builder.WithCommand(args);
    }

    public static TBuilder WithMountedResources<
        TBuilder,
        TContainer,
        TConfiguration>(
        this TBuilder builder,
        IEnumerable<ContainerResourceMapping> resources)
    where TContainer : IContainer
    where TConfiguration : IContainerConfiguration
    where TBuilder : ContainerBuilder<
        TBuilder,
        TContainer,
        TConfiguration>
    {
        foreach (ContainerResourceMapping resource in resources)
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
