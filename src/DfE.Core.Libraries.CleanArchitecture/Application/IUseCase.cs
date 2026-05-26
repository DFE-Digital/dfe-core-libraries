namespace DfE.Core.Libraries.CleanArchitecture.Application;
/// <summary>
/// Defines a use-case with an input request and an expected response.
/// </summary>
/// <typeparam name="TUseCaseRequest">
/// The type of request (input port) passed to a use case.</typeparam>
/// <typeparam name="TUseCaseResponse">
/// The type of response (output) returned from the use case.
/// </typeparam>
public interface IUseCase<in TUseCaseRequest, TUseCaseResponse>
    where TUseCaseRequest : IUseCaseRequest<TUseCaseResponse>
    where TUseCaseResponse : class
{
    /// <summary>
    /// Handles the request and returns the response.
    /// </summary>
    /// <param name="request">The input request.</param>
    /// <returns>The output response object.</returns>
    Task<TUseCaseResponse> HandleRequestAsync(TUseCaseRequest request, CancellationToken cancellationToken = default);
}
