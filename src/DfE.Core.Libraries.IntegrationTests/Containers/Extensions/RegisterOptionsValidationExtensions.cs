using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Container;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;

public static class RegisterOptionsValidationExtensions
{
    public static IServiceCollection AddContainerOptionsValidation(this IServiceCollection services)
    {
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<
                IValidateOptions<ContainerOptions>,
                ContainerOptionsValidator>());

        return services;
    }
}
