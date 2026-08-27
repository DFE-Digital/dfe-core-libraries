using DfE.Core.Libraries.Testing;
using DfE.Core.Libraries.Testing.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions;

public abstract class ServiceProviderTestsBase : IntegrationTestsBase
{
    private IServiceProvider? _applicationServicesRootProvider;

    protected ServiceProviderTestsBase(IServiceProvider testServicesProvider)
    {
        TestServicesProvider = testServicesProvider ?? throw new ArgumentNullException(nameof(testServicesProvider));
    }

    protected IServiceProvider TestServicesProvider { get; }

    protected IServiceProvider ApplicationServicesRootProvider =>
        _applicationServicesRootProvider ??
            throw new InvalidOperationException("Application services have not been initialised");

    protected sealed override Task StartApplicationAsync(
        CancellationToken ct = default)
    {
        if (_applicationServicesRootProvider is not null)
        {
            throw new InvalidOperationException("Application services have already been initialised");
        }

        _applicationServicesRootProvider =
            BuildApplicationServices(
                configuration: MergeTestAndApplicationConfiguration(),
                configure: ConfigureApplicationServices);

        return Task.CompletedTask;
    }

    protected sealed override async Task DisposeApplicationAsync()
    {
        if (_applicationServicesRootProvider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (_applicationServicesRootProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    protected virtual void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration) { }

    protected virtual void ConfigureApplicationConfiguration(IConfigurationBuilder builder) { }

    protected TSingletonService ResolveSingletonApplicationService<TSingletonService>()
        where TSingletonService : notnull
    {
        return ApplicationServicesRootProvider
            .GetRequiredService<TSingletonService>();
    }

    protected async Task<TResult> RunScopedAsync<TService, TResult>(
        Func<TService, Task<TResult>> action)
        where TService : notnull
    {
        using IServiceScope scope =
            ApplicationServicesRootProvider.CreateScope();

        TService service =
            scope.ServiceProvider.GetRequiredService<TService>();

        return await action(service);
    }

    private IConfiguration MergeTestAndApplicationConfiguration()
    {
        // test config
        IConfigurationBuilder builder =
            ConfigurationDefault.CreateBuilder()
                .AddConfiguration(
                    TestServicesProvider.GetRequiredService<IConfiguration>());

        // application config override
        ConfigureApplicationConfiguration(builder);

        return builder.Build();
    }

    private static IServiceProvider BuildApplicationServices(
        IConfiguration configuration,
        Action<IServiceCollection, IConfiguration>? configure = null)
    {
        IServiceCollection services = ServiceCollectionDefaults.Create();

        configure?.Invoke(services, configuration);

        services.AddSingleton(configuration);

        return services.BuildServiceProvider(ServiceProviderOptionsDefaults.Default);
    }
}
