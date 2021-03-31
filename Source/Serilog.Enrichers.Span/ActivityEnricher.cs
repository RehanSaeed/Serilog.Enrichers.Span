namespace Serilog.Enrichers.Span
{
    using System.Diagnostics;
    using Serilog.Core;
    using Serilog.Events;

    /// <summary>
    /// A log event enricher which adds span information from the current <see cref="Activity"/>.
    /// </summary>
    public class ActivityEnricher : ILogEventEnricher
    {
#if NET5_0
        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var activity = Activity.Current;

            if (activity is not null)
            {
                AddSpanId(logEvent, activity);
                AddTraceId(logEvent, activity);
                AddParentId(logEvent, activity);
            }
        }

        private static void AddSpanId(LogEvent logEvent, Activity activity)
        {
            var property = activity.GetCustomProperty("Serilog.SpanId");
            if (property is null || property is not LogEventProperty logEventProperty)
            {
                logEventProperty = new LogEventProperty("SpanId", new ScalarValue(activity.GetSpanId()));
                activity.SetCustomProperty("Serilog.SpanId", logEventProperty);
            }

            logEvent.AddPropertyIfAbsent(logEventProperty);
        }

        private static void AddTraceId(LogEvent logEvent, Activity activity)
        {
            var property = activity.GetCustomProperty("Serilog.TraceId");
            if (property is null || property is not LogEventProperty logEventProperty)
            {
                logEventProperty = new LogEventProperty("TraceId", new ScalarValue(activity.GetTraceId()));
                activity.SetCustomProperty("Serilog.TraceId", logEventProperty);
            }

            logEvent.AddPropertyIfAbsent(logEventProperty);
        }

        private static void AddParentId(LogEvent logEvent, Activity activity)
        {
            var property = activity.GetCustomProperty("Serilog.ParentId");
            if (property is null || property is not LogEventProperty logEventProperty)
            {
                logEventProperty = new LogEventProperty("ParentId", new ScalarValue(activity.GetParentId()));
                activity.SetCustomProperty("Serilog.ParentId", logEventProperty);
            }

            logEvent.AddPropertyIfAbsent(logEventProperty);
        }
#elif NETCOREAPP3_0 || NETCOREAPP3_1
        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var activity = Activity.Current;

            if (activity is not null)
            {
                logEvent.AddPropertyIfAbsent(new LogEventProperty("SpanId", new ScalarValue(activity.GetSpanId())));
                logEvent.AddPropertyIfAbsent(new LogEventProperty("TraceId", new ScalarValue(activity.GetTraceId())));
                logEvent.AddPropertyIfAbsent(new LogEventProperty("ParentId", new ScalarValue(activity.GetParentId())));
            }
        }
#endif
    }
}
