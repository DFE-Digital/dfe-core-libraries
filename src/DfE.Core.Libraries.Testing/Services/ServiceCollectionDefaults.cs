using Microsoft.Extensions.DependencyInjection;

namespace DfE.Core.Libraries.Testing.Services;

public static class ServiceCollectionDefaults
{
    public static IServiceCollection Create() => new ServiceCollection();

}
