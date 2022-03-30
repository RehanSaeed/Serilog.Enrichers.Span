namespace Serilog.Enrichers.Span;

using System;
using System.Diagnostics;
using System.Linq;
using Serilog.Core;
using Serilog.Events;

/// <summary>
/// A log event enricher which adds baggage from the current <see cref="Activity"/>.
/// </summary>
public class ActivityBaggageEnricher : ILogEventEnricher
{
    private const string BaggageTags = "Baggage";

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

        var activity = Activity.Current;
        if (activity is not null)
        {
            var baggage = activity.Baggage.Select(bag => new LogEventProperty(bag.Key, new ScalarValue(bag.Value)));
            if (baggage.Any())
            {
                logEvent.AddPropertyIfAbsent(new LogEventProperty(BaggageTags, new StructureValue(baggage)));
            }
        }
    }
}
