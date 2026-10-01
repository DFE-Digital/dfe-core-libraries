namespace DfE.Core.Libraries.CleanArchitecture.Application;

/// <summary>
/// Represents the outcome of a typical use-case operation,
/// including success/failure state, an model object, and error information.
/// </summary>
/// <typeparam name="TModel">
/// The type of the model returned when the operation succeeds.
/// </typeparam>
/// <remarks>
/// This class provides a simple way to encapsulate the result of a use case:
/// <list type="bullet">
///   <item><description><see cref="Success(TModel)"/> for successful operations with a model result.</description></item>
///   <item><description><see cref="Failure(string)"/> for failed operations with an error message.</description></item>
/// </list>
/// Consumers should check <see cref="SuccessfulRequest"/> before using the <see cref="Model"/> property. />
/// </remarks>
public sealed class UseCaseResponse<TModel> where TModel : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UseCaseResponse{TModel}"/> class.
    /// </summary>
    /// <param name="successfulRequest">Indicates whether the operation succeeded.</param>
    /// <param name="model">The model returned by the operation, if any.</param>
    /// <param name="errorMessage">The error message if the operation failed.</param>
    private UseCaseResponse(
        bool successfulRequest, TModel model, string? errorMessage)
    {
        SuccessfulRequest = successfulRequest;
        Model = model ?? throw new ArgumentNullException(nameof(model));
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result with the specified model value.
    /// </summary>
    /// <param name="model">The model returned by the successful operation.</param>
    /// <returns>
    /// A <see cref="UseCaseResponse{TModel}"/> representing a successful request.
    /// </returns>
    public static UseCaseResponse<TModel> Success(TModel model) =>
        new(successfulRequest: true, model, errorMessage: null);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="error">The error message describing why the operation failed.</param>
    /// <returns>
    /// A <see cref="UseCaseResponse{TModel}"/> representing a failed request.
    /// </returns>
    public static UseCaseResponse<TModel> Failure(TModel model, string error) =>
        new(successfulRequest: false, model: model, error);

    /// <summary>
    /// Indicates whether the operation succeeded.
    /// </summary>
    /// <remarks>
    /// A value of <c>true</c> means the use case completed successfully.
    /// A value of <c>false</c> means the use case failed and an error message
    /// may be available in <see cref="ErrorMessage"/>.
    /// </remarks>
    public bool SuccessfulRequest { get; }

    /// <summary>
    /// Gets the value returned by the operation if successful.
    /// </summary>
    /// <remarks>
    /// This property will always contain the model object</c>.
    /// </remarks>
    public TModel Model { get; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    /// <remarks>
    /// This property will contain a descriptive error message when <see cref="SuccessfulRequest"/> is <c>false</c>.
    /// It will be <c>null</c> if the operation succeeded.
    /// </remarks>
    public string? ErrorMessage { get; }
}
