namespace Serilog.Enrichers.Span;

using System;
using System.Collections.Generic;
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
    private readonly SpanLogEventPropertiesNames propertiesNames;
    private readonly ISpanLogEventPropertyAugmentor? logEventPropertyAugmentor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityEnricher"/> class with default event properties names.
    /// </summary>
    /// <param name="logEventPropertyAugmentor">Augmentor to apply to trace values added to the log event.</param>
    public ActivityEnricher(ISpanLogEventPropertyAugmentor? logEventPropertyAugmentor = null)
    {
        this.propertiesNames = new SpanLogEventPropertiesNames();
        this.logEventPropertyAugmentor = logEventPropertyAugmentor;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityEnricher"/> class.
    /// </summary>
    /// <param name="logEventPropertiesNames">Names for log event properties.</param>
    /// <param name="logEventPropertyAugmentor">Augmentor to apply to trace values added to the log event.</param>
    public ActivityEnricher(SpanLogEventPropertiesNames logEventPropertiesNames, ISpanLogEventPropertyAugmentor? logEventPropertyAugmentor = null)
    {
        CheckPropertiesNamesArgument(logEventPropertiesNames);
        this.propertiesNames = logEventPropertiesNames;
        this.logEventPropertyAugmentor = logEventPropertyAugmentor;
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
            var spanId = activity.GetSpanId();
            foreach (var additionalProperty in this.AugmentSpanId(activity))
            {
                logEvent.AddPropertyIfAbsent(additionalProperty);
            }

            logEvent.AddPropertyIfAbsent(new LogEventProperty(
                this.propertiesNames.SpanId,
                new ScalarValue(spanId)));

            var traceId = activity.GetTraceId();
            foreach (var additionalProperty in this.AugmentTraceId(activity))
            {
                logEvent.AddPropertyIfAbsent(additionalProperty);
            }

            logEvent.AddPropertyIfAbsent(new LogEventProperty(
                this.propertiesNames.TraceId,
                new ScalarValue(traceId)));

            var parentId = activity.GetParentId();
            foreach (var additionalProperty in this.AugmentParentId(activity))
            {
                logEvent.AddPropertyIfAbsent(additionalProperty);
            }

            logEvent.AddPropertyIfAbsent(new LogEventProperty(
                this.propertiesNames.ParentId,
                new ScalarValue(parentId)));
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
        var property = activity.GetCustomProperty(SpanIdKey);
        if (property is null || property is not LogEventProperty logEventProperty)
        {
            logEventProperty = new LogEventProperty(this.propertiesNames.SpanId, new ScalarValue(activity.GetSpanId()));
            activity.SetCustomProperty(SpanIdKey, logEventProperty);
        }

        foreach (var additionalProperty in this.AugmentSpanId(activity))
        {
            logEvent.AddPropertyIfAbsent(additionalProperty);
        }

        logEvent.AddPropertyIfAbsent(logEventProperty);
    }

    private void AddTraceId(LogEvent logEvent, Activity activity)
    {
        var property = activity.GetCustomProperty(TraceIdKey);
        if (property is null || property is not LogEventProperty logEventProperty)
        {
            logEventProperty =
                new LogEventProperty(this.propertiesNames.TraceId, new ScalarValue(activity.GetTraceId()));
            activity.SetCustomProperty(TraceIdKey, logEventProperty);
        }

        foreach (var additionalProperty in this.AugmentTraceId(activity))
        {
            logEvent.AddPropertyIfAbsent(additionalProperty);
        }

        logEvent.AddPropertyIfAbsent(logEventProperty);
    }

    private void AddParentId(LogEvent logEvent, Activity activity)
    {
        var property = activity.GetCustomProperty(ParentIdKey);
        if (property is null || property is not LogEventProperty logEventProperty)
        {
            logEventProperty =
                new LogEventProperty(this.propertiesNames.ParentId, new ScalarValue(activity.GetParentId()));
            activity.SetCustomProperty(ParentIdKey, logEventProperty);
        }

        foreach (var additionalProperty in this.AugmentParentId(activity))
        {
            logEvent.AddPropertyIfAbsent(additionalProperty);
        }

        logEvent.AddPropertyIfAbsent(logEventProperty);
    }
#endif

    private IEnumerable<LogEventProperty> AugmentSpanId(Activity activity)
    {
        var spanId = activity.GetSpanId();

        if (this.logEventPropertyAugmentor == null)
        {
            return Array.Empty<LogEventProperty>();
        }

        return this.logEventPropertyAugmentor.AugmentSpanId(spanId);
    }

    private IEnumerable<LogEventProperty> AugmentTraceId(Activity activity)
    {
        var traceId = activity.GetTraceId();

        if (this.logEventPropertyAugmentor == null)
        {
            return Array.Empty<LogEventProperty>();
        }

        return this.logEventPropertyAugmentor.AugmentTraceId(traceId);
    }

    private IEnumerable<LogEventProperty> AugmentParentId(Activity activity)
    {
        var parentId = activity.GetParentId();

        if (this.logEventPropertyAugmentor == null)
        {
            return Array.Empty<LogEventProperty>();
        }

        return this.logEventPropertyAugmentor.AugmentParentId(parentId);
    }
}
