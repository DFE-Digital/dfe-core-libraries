using Microsoft.Extensions.Options;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Extensions;

public static class ValidationExtensions
{
    public static void ThrowIfFailed<T>(
        this ValidateOptionsResult result,
        string? name) where T : class
    {
        if (result.Failed)
        {
            throw new OptionsValidationException(
                name!,
                typeof(T),
                result.Failures);
        }
    }
}
