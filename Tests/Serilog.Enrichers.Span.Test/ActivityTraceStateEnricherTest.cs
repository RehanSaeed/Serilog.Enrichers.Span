namespace Serilog.Enrichers.Span.Test;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Moq;
using Serilog.Core;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Parsing;
using Xunit;

public class ActivityTraceStateEnricherTest
{
    private static readonly ActivitySource Source = new("Sample.DistributedTracing", "1.0.0");

    private static readonly ActivityListener Listener = new()
    {
        ShouldListenTo = _ => true,
        Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
    };

    public ActivityTraceStateEnricherTest() => ActivitySource.AddActivityListener(Listener);

    [Fact]
    public void Given_activity_with_trace_state_When_log_event_created_Then_log_event_contains_tracestate()
    {
        using var act = Source.StartActivity();
        if (act is not null)
        {
            act.TraceStateString = "test=val";
        }

        var class1 = new ActivityTraceStateEnricher();
        var @event = MakeLogEvent();
        class1.Enrich(@event, Mock.Of<ILogEventPropertyFactory>());

        var eventPropertiesNames = @event.Properties.Keys;
        Assert.Collection(
            eventPropertiesNames,
            s => Assert.Equal("TraceState", s));
    }

    private static LogEvent MakeLogEvent() =>
        new(
            DateTimeOffset.Now,
            LogEventLevel.Information,
            null,
            new MessageTemplate(
                new List<MessageTemplateToken>()),
            new List<LogEventProperty>());
}
