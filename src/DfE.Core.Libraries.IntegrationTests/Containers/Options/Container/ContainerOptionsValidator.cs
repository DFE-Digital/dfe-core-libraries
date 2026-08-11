using Microsoft.Extensions.Options;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Container;

internal sealed class ContainerOptionsValidator : IValidateOptions<ContainerOptions>
{
    public ValidateOptionsResult Validate(string? name, ContainerOptions options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail("Container options cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(options.Image))
        {
            return ValidateOptionsResult.Fail("Container image cannot be null or whitespace.");
        }

        return ValidateOptionsResult.Success;
    }
}
