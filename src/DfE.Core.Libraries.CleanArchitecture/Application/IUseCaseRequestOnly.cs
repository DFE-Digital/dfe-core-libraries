namespace DfE.Core.Libraries.CleanArchitecture.Application;

/// <summary>
/// Defines a use-case where only a request is provided, but no response is returned.
/// Useful for commands or operations without direct output.
/// </summary>
/// <typeparam name="TUseCaseRequest">
/// The output response type expected to be returned by the use case.
/// </typeparam>
public partial interface IUseCaseRequestOnly<in TUseCaseRequest>
    where TUseCaseRequest : IUseCaseRequest
{
    /// <summary>
    /// Handles the request without returning a response.
    /// </summary>
    /// <param name="request">The input request.</param>
    Task HandleRequestAsync(TUseCaseRequest request, CancellationToken cancellationToken = default);
}
