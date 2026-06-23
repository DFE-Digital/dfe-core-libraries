using Microsoft.Extensions.DependencyInjection;

namespace DfE.Core.Libraries.Testing.Services;

public static class ServiceProviderOptionsDefaults
{
    public static ServiceProviderOptions Default => new()
    {
        ValidateOnBuild = true,
        ValidateScopes = true
    };
}
