namespace Serilog.Enrichers.Span;

using System;

/// <summary>
/// Names for log event properties of span.
/// </summary>
public class SpanLogEventPropertiesNames
{
    private string parentId = "ParentId";
    private string traceId = "TraceId";
    private string spanId = "SpanId";
    private string operationName = "OperationName";

    /// <summary>
    /// Gets or sets a name for trace id property.
    /// </summary>
    public string TraceId
    {
        get => this.traceId;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The property must not be empty", nameof(this.TraceId));
            }

            this.traceId = value;
        }
    }

    /// <summary>
    /// Gets or sets a name for parent id property.
    /// </summary>
    public string ParentId
    {
        get => this.parentId;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The property must not be empty", nameof(this.ParentId));
            }

            this.parentId = value;
        }
    }

    /// <summary>
    /// Gets or sets a name for span id property.
    /// </summary>
    public string SpanId
    {
        get => this.spanId;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The property must not be empty", nameof(this.SpanId));
            }

            this.spanId = value;
        }
    }

    /// <summary>
    /// Gets or sets a name for operation name property.
    /// </summary>
    public string OperationName
    {
        get => this.operationName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The property must not be empty", nameof(this.OperationName));
            }

            this.operationName = value;
        }
    }
}
