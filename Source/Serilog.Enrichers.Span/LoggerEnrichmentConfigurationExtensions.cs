namespace Serilog.Enrichers.Span;

using System;
using System.Diagnostics;
using Serilog.Configuration;

/// <summary>
/// <see cref="LoggerEnrichmentConfiguration"/> extension methods.
/// </summary>
public static class LoggerEnrichmentConfigurationExtensions
{
    /// <summary>
    /// Enrich logger output with span information from the current <see cref="Activity"/>.
    /// </summary>
    /// <param name="loggerEnrichmentConfiguration">The enrichment configuration.</param>
    /// <returns>Configuration object allowing method chaining.</returns>
    public static LoggerConfiguration WithSpan(this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration) =>
        loggerEnrichmentConfiguration.WithSpan(new SpanOptions());

    /// <summary>
    /// Enrich logger output with span information from the current <see cref="Activity"/>.
    /// </summary>
    /// <param name="loggerEnrichmentConfiguration">The enrichment configuration.</param>
    /// <param name="spanOptions"><see cref="SpanOptions"/> to use for enriching Activity.</param>
    /// <returns>Configuration object allowing method chaining.</returns>
    public static LoggerConfiguration WithSpan(this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration, SpanOptions spanOptions)
    {
        ArgumentNullException.ThrowIfNull(loggerEnrichmentConfiguration);
        ArgumentNullException.ThrowIfNull(spanOptions);

        if (spanOptions.IncludeTags)
        {
            loggerEnrichmentConfiguration.With<ActivityTagEnricher>();
        }

        return loggerEnrichmentConfiguration.With<ActivityEnricher>();
    }
}
