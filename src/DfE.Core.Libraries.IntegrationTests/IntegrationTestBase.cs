namespace DfE.Core.Libraries.IntegrationTests.Abstractions;

public abstract class IntegrationTestsBase : IAsyncDisposable
{
    private bool _started;

    protected async ValueTask StartTestAsync(
        CancellationToken ct = default)
    {
        if (_started)
        {
            throw new InvalidOperationException("Test has already started");
        }

        _started = true;

        await BeforeStartTestDependenciesAsync(ct);

        await StartTestDependenciesAsync(ct);

        await AfterStartTestDependenciesAsync(ct);

        await StartApplicationAsync(ct);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await BeforeDisposeAsync();

        await DisposeApplicationAsync();

        await AfterDisposeAsync();
    }

    protected virtual Task BeforeStartTestDependenciesAsync(CancellationToken ct = default) => Task.CompletedTask;

    protected virtual Task StartTestDependenciesAsync(CancellationToken ct = default) => Task.CompletedTask;

    protected virtual Task AfterStartTestDependenciesAsync(CancellationToken ct = default) => Task.CompletedTask;

    protected virtual Task StartApplicationAsync(CancellationToken ct = default) => Task.CompletedTask;

    protected virtual Task DisposeApplicationAsync() => Task.CompletedTask;

    protected virtual Task BeforeDisposeAsync() => Task.CompletedTask;

    protected virtual Task AfterDisposeAsync() => Task.CompletedTask;
}
