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
    private const string SpanIdKeyPrefix = "Serilog.SpanId.";
    private const string TraceIdKeyPrefix = "Serilog.TraceId.";
    private const string ParentIdKeyPrefix = "Serilog.ParentId.";

    private readonly string spanIdKey;
    private readonly string traceIdKey;
    private readonly string parentIdKey;
#endif
    private readonly SpanLogEventPropertiesNames propertiesNames;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityEnricher"/> class with default event properties names.
    /// </summary>
    public ActivityEnricher()
        : this(new SpanLogEventPropertiesNames())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityEnricher"/> class.
    /// </summary>
    /// <param name="logEventPropertiesNames">Names for log event properties.</param>
    public ActivityEnricher(SpanLogEventPropertiesNames logEventPropertiesNames)
    {
        CheckPropertiesNamesArgument(logEventPropertiesNames);
        this.propertiesNames = logEventPropertiesNames;
#if NET5_0_OR_GREATER
        this.parentIdKey = ParentIdKeyPrefix + logEventPropertiesNames.ParentId;
        this.spanIdKey = SpanIdKeyPrefix + logEventPropertiesNames.SpanId;
        this.traceIdKey = TraceIdKeyPrefix + logEventPropertiesNames.TraceId;
#endif
    }

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
            this.AddSpanId(logEvent, activity);
            this.AddTraceId(logEvent, activity);
            this.AddParentId(logEvent, activity);
#else
            logEvent.AddPropertyIfAbsent(new LogEventProperty(
                this.propertiesNames.SpanId,
                new ScalarValue(activity.GetSpanId())));
            logEvent.AddPropertyIfAbsent(new LogEventProperty(
                this.propertiesNames.TraceId,
                new ScalarValue(activity.GetTraceId())));
            logEvent.AddPropertyIfAbsent(new LogEventProperty(
                this.propertiesNames.ParentId,
                new ScalarValue(activity.GetParentId())));
#endif
        }
    }

    private static void CheckPropertiesNamesArgument(SpanLogEventPropertiesNames logEventPropertyNames)
    {
#if NET6_0_OR_GREATER
#pragma warning disable IDE0022
        ArgumentNullException.ThrowIfNull(logEventPropertyNames);
#pragma warning restore IDE0022
#else
        if (logEventPropertyNames is null)
        {
            throw new ArgumentNullException(nameof(logEventPropertyNames));
        }
#endif
    }

#if NET5_0_OR_GREATER
    private void AddSpanId(LogEvent logEvent, Activity activity)
    {
        var property = activity.GetCustomProperty(this.spanIdKey);
        if (property is null || property is not LogEventProperty logEventProperty)
        {
            logEventProperty = new LogEventProperty(this.propertiesNames.SpanId, new ScalarValue(activity.GetSpanId()));
            activity.SetCustomProperty(this.spanIdKey, logEventProperty);
        }

        logEvent.AddPropertyIfAbsent(logEventProperty);
    }

    private void AddTraceId(LogEvent logEvent, Activity activity)
    {
        var property = activity.GetCustomProperty(this.traceIdKey);
        if (property is null || property is not LogEventProperty logEventProperty)
        {
            logEventProperty =
                new LogEventProperty(this.propertiesNames.TraceId, new ScalarValue(activity.GetTraceId()));
            activity.SetCustomProperty(this.traceIdKey, logEventProperty);
        }

        logEvent.AddPropertyIfAbsent(logEventProperty);
    }

    private void AddParentId(LogEvent logEvent, Activity activity)
    {
        var property = activity.GetCustomProperty(this.parentIdKey);
        if (property is null || property is not LogEventProperty logEventProperty)
        {
            logEventProperty =
                new LogEventProperty(this.propertiesNames.ParentId, new ScalarValue(activity.GetParentId()));
            activity.SetCustomProperty(this.parentIdKey, logEventProperty);
        }

        logEvent.AddPropertyIfAbsent(logEventProperty);
    }
#endif
}
