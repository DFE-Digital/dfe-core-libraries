using Microsoft.Extensions.Logging;

namespace DfE.Core.Libraries.Testing.Logger;

public sealed class InMemoryLogger<TLogCategory> : ILogger<TLogCategory>
{
    public List<LoggedMessage> Logs { get; } = [];

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        Logs.Add(
            new LoggedMessage(
                formatter(state, exception),
                logLevel));
    }
}

internal sealed class NullScope : IDisposable
{
    public static NullScope Instance { get; } = new();
    public void Dispose() { }
}
