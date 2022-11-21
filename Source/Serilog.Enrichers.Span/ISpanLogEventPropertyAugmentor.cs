namespace Serilog.Enrichers.Span;

using System.Collections.Generic;
using Serilog.Events;

/// <summary>
/// Generates additional log event properties to augment the representation of the trace context of a log message. This
/// can be used to create additional log event properties with alternative names or representations of property values
/// to satisfy alternative tracing conventions.
/// </summary>
public interface ISpanLogEventPropertyAugmentor
{
    /// <summary>
    /// Generates additional log event properties to augment the representation of the TraceId.
    /// </summary>
    /// <param name="traceId">The value of the TraceId to augment.</param>
    /// <returns>Zero or more additional log event properties to represent the TraceId.</returns>
    IEnumerable<LogEventProperty> AugmentTraceId(string traceId);

    /// <summary>
    /// Generates additional log event properties to augment the representation of the SpanId.
    /// </summary>
    /// <param name="spanId">The value of the SpanId to augment.</param>
    /// <returns>Zero or more additional log event properties to represent the SpanId.</returns>
    IEnumerable<LogEventProperty> AugmentSpanId(string spanId);

    /// <summary>
    /// Generates additional log event properties to augment the representation of the ParentId.
    /// </summary>
    /// <param name="parentId">The value of the ParentId to augment.</param>
    /// <returns>Zero or more additional log event properties to represent the ParentId.</returns>
    IEnumerable<LogEventProperty> AugmentParentId(string parentId);
}
