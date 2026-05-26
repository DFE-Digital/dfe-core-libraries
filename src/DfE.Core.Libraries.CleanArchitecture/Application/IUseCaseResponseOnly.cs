namespace DfE.Core.Libraries.CleanArchitecture.Application;

/// <summary>
/// Defines a use-case where only no request is provided, but a response is returned.
/// </summary>
/// <typeparam name="TUseCaseResponse">
/// The output response type expected to be returned by the use case.
/// </typeparam>
public partial interface IUseCaseResponseOnly<TUseCaseResponse>
    where TUseCaseResponse : class
{
    /// <summary>
    /// Handles the no request case and returns a response.
    /// </summary>
    /// <returns>The output response.</returns>
    Task<TUseCaseResponse> HandleRequestAsync(CancellationToken cancellationToken = default);
}
