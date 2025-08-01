using MAS_Shared.Models;
using Microsoft.Extensions.Logging;
using SidekickApp.Services;

public class UILoggerProvider : ILoggerProvider
{
    private readonly UILogSink _sink;

    public UILoggerProvider(UILogSink sink)
    {
        _sink = sink;
    }

    public ILogger CreateLogger(string categoryName) => new UILogger(_sink, categoryName);

    public void Dispose() { }
}

public class UILogger : ILogger
{
    private readonly UILogSink _sink;
    private readonly string _category;

    public UILogger(UILogSink sink, string category)
    {
        _sink = sink;
        _category = category;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return UILoggerNullScope.Instance;
    }
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId,
        TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        _sink.Add(logLevel, _category, message, exception);
    }
}