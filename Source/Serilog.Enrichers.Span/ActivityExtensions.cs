namespace Serilog.Enrichers.Span;

using System;
using System.Diagnostics;

/// <summary>
/// <see cref="Activity"/> extension methods.
/// </summary>
[CLSCompliant(false)]
public static class ActivityExtensions
{
    /// <summary>
    /// Gets the span unique identifier regardless of the activity identifier format.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>The span unique identifier.</returns>
    public static string GetSpanId(this Activity activity)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(activity);
#else
        if (activity is null)
        {
            throw new ArgumentNullException(nameof(activity));
        }
#endif

        var spanId = activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.Id,
            ActivityIdFormat.W3C => activity.SpanId.ToHexString(),
            ActivityIdFormat.Unknown => null,
            _ => null,
        };

        return spanId ?? string.Empty;
    }

    /// <summary>
    /// Gets the span trace unique identifier regardless of the activity identifier format.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>The span trace unique identifier.</returns>
    public static string GetTraceId(this Activity activity)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(activity);
#else
        if (activity is null)
        {
            throw new ArgumentNullException(nameof(activity));
        }
#endif

        var traceId = activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.RootId,
            ActivityIdFormat.W3C => activity.TraceId.ToHexString(),
            ActivityIdFormat.Unknown => null,
            _ => null,
        };

        return traceId ?? string.Empty;
    }

    /// <summary>
    /// Gets the span parent unique identifier regardless of the activity identifier format.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>The span parent unique identifier.</returns>
    public static string GetParentId(this Activity activity)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(activity);
#else
        if (activity is null)
        {
            throw new ArgumentNullException(nameof(activity));
        }
#endif

        var parentId = activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.ParentId,
            ActivityIdFormat.W3C => activity.ParentSpanId.ToHexString(),
            ActivityIdFormat.Unknown => null,
            _ => null,
        };

        return parentId ?? string.Empty;
    }
}
