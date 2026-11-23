using Microsoft.Extensions.Logging;

namespace DfE.Core.Libraries.Testing.Logger;

public sealed class LoggedMessage
{
    public LoggedMessage(string message, LogLevel level)
    {
        Message = message;
        Level = level;
    }

    public string Message { get; }

    public LogLevel Level { get; }
}
