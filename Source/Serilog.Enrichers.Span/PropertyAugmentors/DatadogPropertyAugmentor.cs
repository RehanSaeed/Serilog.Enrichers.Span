namespace Serilog.Enrichers.Span.TraceFormatConverters;

using System;
using System.Collections.Generic;
using System.Globalization;
using Serilog.Events;

/// <summary>
/// Converts OTEL trace values to the format expected by Datadog.
/// </summary>
/// <remarks>
/// OTEL trace and span IDs are 16-byte and 8-byte values respectively and are commonly represented as hex strings, but
/// Datadog expects 64-bit decimal values for both. This converter implements the recommendation from Datadog's
/// documentation to represent the right-most 8 bytes of each value as a 64-bit integer.
/// </remarks>
public class DatadogPropertyAugmentor : ISpanLogEventPropertyAugmentor
{
    /// <summary>
    /// The default field name to use for generated trace ID properties.
    /// </summary>
    public const string DefaultTraceIdName = "dd.trace_id";

    /// <summary>
    /// The default field name to use for generated span ID properties.
    /// </summary>
    public const string DefaultSpanIdName = "dd.span_id";

    private readonly string traceIdName;
    private readonly string spanIdName;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatadogPropertyAugmentor"/> class.
    /// </summary>
    /// <param name="traceIdName">The property name to use for generated trace IDs.</param>
    /// <param name="spanIdName">The property name to use for generated span IDs.</param>
    public DatadogPropertyAugmentor(
        string traceIdName = DefaultTraceIdName,
        string spanIdName = DefaultSpanIdName)
    {
        if (string.IsNullOrWhiteSpace(traceIdName))
        {
            throw new ArgumentException("The value must not be empty", nameof(traceIdName));
        }

        if (string.IsNullOrWhiteSpace(spanIdName))
        {
            throw new ArgumentException("The value must not be empty", nameof(spanIdName));
        }

        this.traceIdName = traceIdName;
        this.spanIdName = spanIdName;
    }

    /// <summary>
    /// Converts 16-byte hex string values into decimal representations of the right-most 8 bytes.
    /// </summary>
    /// <param name="traceId">The trace ID to convert.</param>
    /// <returns>
    /// A <see cref="LogEventProperty"/> containing then decimal representation of the right-most 8 bytes of
    /// <paramref name="traceId"/> if its value is a 16-byte hex string, otherwise an empty <see cref="IEnumerable{T}"/>.
    /// </returns>
    public IEnumerable<LogEventProperty> AugmentTraceId(string traceId)
    {
        if (traceId.Length != 32)
        {
            return Array.Empty<LogEventProperty>();
        }

        try
        {
#if NET6_0_OR_GREATER
            var convertedTraceId = ConvertHexToDecimal(traceId[16..]);
#else
            var convertedTraceId = ConvertHexToDecimal(traceId.Substring(16));
#endif

            return new[]
            {
                new LogEventProperty(this.traceIdName, new ScalarValue(convertedTraceId)),
            };
        }
        catch
        {
            return Array.Empty<LogEventProperty>();
        }
    }

    /// <summary>
    /// Converts 8-byte hex string values into decimal representation.
    /// </summary>
    /// <param name="spanId">The span ID to convert.</param>
    /// <returns>
    /// A <see cref="LogEventProperty"/> containing the decimal representation of <paramref name="spanId"/> if its value
    /// is an 8-byte hex string, otherwise an empty <see cref="IEnumerable{T}"/>.
    /// </returns>
    public IEnumerable<LogEventProperty> AugmentSpanId(string spanId)
    {
        if (spanId.Length != 16)
        {
            return Array.Empty<LogEventProperty>();
        }

        try
        {
            return new[]
            {
                new LogEventProperty(this.spanIdName, new ScalarValue(ConvertHexToDecimal(spanId))),
            };
        }
        catch
        {
            return Array.Empty<LogEventProperty>();
        }
    }

    /// <summary>
    /// Returns an empty <see cref="IEnumerable{T}"/> because Datadog has no expectation about parent IDs in logs.
    /// </summary>
    /// <param name="parentId">The parent ID to convert.</param>
    /// <returns>An empty <see cref="IEnumerable{T}"/> of <see cref="LogEvent"/>.</returns>
    public IEnumerable<LogEventProperty> AugmentParentId(string parentId) => Array.Empty<LogEventProperty>();

    private static string ConvertHexToDecimal(string maybeHex) =>
        Convert.ToUInt64(maybeHex, fromBase: 16).ToString("D", new CultureInfo("en-us"));
}
