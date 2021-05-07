namespace Serilog.Enrichers.Span.Test
{
    using System;
    using Serilog.Core;
    using Serilog.Events;

    public class DelegatingSink : ILogEventSink
    {
        private readonly Action<LogEvent> _write;

        public DelegatingSink(Action<LogEvent> write)
        {
            if (write == null)
                throw new ArgumentNullException(nameof(write));
            _write = write;
        }

        public void Emit(LogEvent logEvent)
        {
            _write(logEvent);
        }

        public static LogEvent? GetLogEvent(Action<ILogger> writeAction)
        {
            LogEvent? result = null;
            var l = new LoggerConfiguration()
                .WriteTo.Sink(new DelegatingSink(le => result = le))
                .CreateLogger();

            writeAction(l);
            return result;
        }
    }
}
