namespace Serilog.Enrichers.Span;

/// <summary>
/// Names for log event properties of span
/// </summary>
public class SpanLogEventPropertiesNames
{
    /// <summary>
    /// Gets or sets a name for trace id property.
    /// </summary>
    public string TraceId { get; set; } = "TraceId";

    /// <summary>
    /// Gets or sets a name for parent id property.
    /// </summary>
    public string ParentId { get; set; } = "ParentId";

    /// <summary>
    /// Gets or sets a name for span id property.
    /// </summary>
    public string SpanId { get; set; } = "SpanId";
}
