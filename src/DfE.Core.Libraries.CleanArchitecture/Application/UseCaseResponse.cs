namespace DfE.Core.Libraries.CleanArchitecture.Application;

/// <summary>
/// Represents the outcome of a typical use-case operation,
/// including success/failure state, an optional model object, and error information.
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
/// Consumers should check both <see cref="SuccessfulRequest"/> and <see cref="HasModel"/> 
/// before using the <see cref="Model"/> property.
/// </remarks>
public sealed class UseCaseResponse<TModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UseCaseResponse{TModel}"/> class.
    /// </summary>
    /// <param name="successfulRequest">Indicates whether the operation succeeded.</param>
    /// <param name="model">The model returned by the operation, if any.</param>
    /// <param name="errorMessage">The error message if the operation failed.</param>
    private UseCaseResponse(
        bool successfulRequest, TModel? model, string? errorMessage)
    {
        SuccessfulRequest = successfulRequest;
        Model = model;
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
    public static UseCaseResponse<TModel> Failure(string error) =>
        new(successfulRequest: false, model: default, error);

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
    /// This property will contain the model object when <see cref="SuccessfulRequest"/> is <c>true</c>.
    /// It will be <c>null</c> if the operation failed or if no model was produced.
    /// </remarks>
    public TModel? Model { get; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    /// <remarks>
    /// This property will contain a descriptive error message when <see cref="SuccessfulRequest"/> is <c>false</c>.
    /// It will be <c>null</c> if the operation succeeded.
    /// </remarks>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Determines whether the current response contains a non-null model.
    /// </summary>
    /// <remarks>
    /// This method is a convenience check that returns <c>true</c> if the
    /// <see cref="Model"/> property is not <c>null</c>, and <c>false</c> otherwise.
    /// It does not consider the <see cref="SuccessfulRequest"/> flag; callers should
    /// check both <see cref="SuccessfulRequest"/> and <see cref="HasModel"/> 
    /// to ensure the response is both successful and contains a usable model.
    /// </remarks>
    /// <returns>
    /// <c>true</c> if the <see cref="Model"/> property is not <c>null</c>;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool HasModel() => Model != null;
}
