using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options;
using Microsoft.Extensions.Options;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Options;

internal sealed class PostgresContainerOptionsValidator
    : IValidateOptions<PostgresContainerOptions>
{
    private readonly IValidateOptions<ContainerOptions> _containerValidator;

    public PostgresContainerOptionsValidator(
        IValidateOptions<ContainerOptions> containerValidator)
    {
        _containerValidator = containerValidator;
    }

    public ValidateOptionsResult Validate(
        string? name,
        PostgresContainerOptions options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(PostgresContainerOptions)} is null.");
        }

        ValidateOptionsResult result =
            options.Container == null ?
                ValidateOptionsResult.Fail("Container options is null") :
                    _containerValidator.Validate(name, options.Container);

        List<string> failures = [];

        if (result.Failed)
        {
            failures.AddRange(result.Failures);
        }

        if (options.Database == null)
        {
            failures.Add("Database options is null");
        }

        if (string.IsNullOrWhiteSpace(options.Database?.Name))
        {
            failures.Add("Database name cannot be null, empty or whitespace.");
        }

        if (string.IsNullOrWhiteSpace(options.Database?.Username))
        {
            failures.Add("Database Username cannot be null, empty or whitespace.");
        }

        if (string.IsNullOrWhiteSpace(options.Database?.Password))
        {
            failures.Add("Database Password cannot be null, empty or whitespace.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
