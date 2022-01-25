namespace Serilog.Enrichers.Span;

using System;
using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

/// <summary>
/// A log event enricher which adds span information from the current <see cref="Activity"/>.
/// </summary>
public class ActivityEnricher : ILogEventEnricher
{
#if NET5_0_OR_GREATER
    private const string SpanIdKey = "Serilog.SpanId";
    private const string TraceIdKey = "Serilog.TraceId";
    private const string ParentIdKey = "Serilog.ParentId";
#endif

    private static SpanLogEventPropertiesNames propertyNames = new();

    /// <summary>
    /// Setup log properties names for span.
    /// </summary>
    /// <param name="logEventPropertiesNames">names for properties.</param>
    public static void SetupLogEventPropertiesNames(SpanLogEventPropertiesNames logEventPropertiesNames) =>
        propertyNames = logEventPropertiesNames;

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
#if NET5_0_OR_GREATER
            AddSpanId(logEvent, activity);
            AddTraceId(logEvent, activity);
            AddParentId(logEvent, activity);
#else
            logEvent.AddPropertyIfAbsent(new LogEventProperty(propertyNames.SpanId, new ScalarValue(activity.GetSpanId())));
            logEvent.AddPropertyIfAbsent(new LogEventProperty(propertyNames.TraceId, new ScalarValue(activity.GetTraceId())));
            logEvent.AddPropertyIfAbsent(new LogEventProperty(propertyNames.ParentId, new ScalarValue(activity.GetParentId())));
#endif
        }
    }
#if NET5_0_OR_GREATER

    private static void AddSpanId(LogEvent logEvent, Activity activity)
    {
        var property = activity.GetCustomProperty(SpanIdKey);
        if (property is null || property is not LogEventProperty logEventProperty)
        {
            logEventProperty = new LogEventProperty(propertyNames.SpanId, new ScalarValue(activity.GetSpanId()));
            activity.SetCustomProperty(SpanIdKey, logEventProperty);
        }

        logEvent.AddPropertyIfAbsent(logEventProperty);
    }

    private static void AddTraceId(LogEvent logEvent, Activity activity)
    {
        var property = activity.GetCustomProperty(TraceIdKey);
        if (property is null || property is not LogEventProperty logEventProperty)
        {
            logEventProperty = new LogEventProperty(propertyNames.TraceId, new ScalarValue(activity.GetTraceId()));
            activity.SetCustomProperty(TraceIdKey, logEventProperty);
        }

        logEvent.AddPropertyIfAbsent(logEventProperty);
    }

    private static void AddParentId(LogEvent logEvent, Activity activity)
    {
        var property = activity.GetCustomProperty(ParentIdKey);
        if (property is null || property is not LogEventProperty logEventProperty)
        {
            logEventProperty = new LogEventProperty(propertyNames.ParentId, new ScalarValue(activity.GetParentId()));
            activity.SetCustomProperty(ParentIdKey, logEventProperty);
        }

        logEvent.AddPropertyIfAbsent(logEventProperty);
    }
#endif

}
