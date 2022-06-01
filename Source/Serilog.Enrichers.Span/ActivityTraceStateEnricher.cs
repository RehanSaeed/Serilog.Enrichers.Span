namespace Serilog.Enrichers.Span;

using System;
using System.Diagnostics;
using System.Linq;
using Serilog.Core;
using Serilog.Events;

/// <summary>
/// A log event enricher which adds trace state from the current <see cref="Activity"/>.
/// </summary>
public class ActivityTraceStateEnricher : ILogEventEnricher
{
    /// <summary>
    /// Enrich the log event.
    /// </summary>
    /// <param name="logEvent">The log event to enrich.</param>
    /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(logEvent);
#else
        if (logEvent is null)
        {
            throw new ArgumentNullException(nameof(logEvent));
        }
#endif

        if (Activity.Current is not null && !string.IsNullOrWhiteSpace(Activity.Current.TraceStateString))
        {
            var state = Activity.Current.TraceStateString?
                .Split(',')
                .Select(p => p.Split('='))
                .Select(p => new LogEventProperty(p.FirstOrDefault(), new ScalarValue(p.Skip(1).FirstOrDefault())));
            logEvent.AddPropertyIfAbsent(new LogEventProperty("TraceState", new StructureValue(state)));
        }
    }
}
