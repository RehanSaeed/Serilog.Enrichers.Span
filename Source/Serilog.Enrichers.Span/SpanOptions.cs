namespace Serilog.Enrichers.Span;

/// <summary>
/// Options.
/// </summary>
public class SpanOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to include tags in log or not. Default is false.
    /// </summary>
    public bool IncludeTags { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include baggage in log or not. Default is false.
    /// </summary>
    public bool IncludeBaggage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include operation name in log or not. Default is false.
    /// </summary>
    public bool IncludeOperationName { get; set; }

    /// <summary>
    /// Gets or sets log properties names for span.
    /// </summary>
    public SpanLogEventPropertiesNames LogEventPropertiesNames { get; set; } = new();

    /// <summary>
    /// Gets or sets the augmentor to apply to trace values added to the log event.
    /// </summary>
    public ISpanLogEventPropertyAugmentor? LogEventPropertyAugmentor { get; set; }
}
