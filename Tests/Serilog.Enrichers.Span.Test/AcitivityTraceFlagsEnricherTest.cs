namespace Serilog.Enrichers.Span.Test;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Moq;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;
using Xunit;

public class AcitivityTraceFlagsEnricherTest
{
    private static readonly ActivitySource Source = new("Sample.DistributedTracing", "1.0.0");

    private static readonly ActivityListener Listener = new()
    {
        ShouldListenTo = _ => true,
    };

    public AcitivityTraceFlagsEnricherTest() => ActivitySource.AddActivityListener(Listener);

    [Fact]
    public void Test_TraceFlags_Recorded()
    {
        Listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        using var act = Source.StartActivity();
        var class1 = new ActivityTraceFlagsEnricher();
        var @event = MakeLogEvent();
        class1.Enrich(@event, Mock.Of<ILogEventPropertyFactory>());

        var eventPropertiesNames = @event.Properties.Keys;
        var eventPropertiesValues = @event.Properties.Values;
        Assert.Collection(eventPropertiesNames, s => Assert.Equal("TraceFlags", s));
        Assert.Collection(eventPropertiesValues, s => Assert.Equal("Recorded", s.ToString()));
    }

    [Fact]
    public void Test_TraceFlags_None()
    {
        Listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        using var act = Source.StartActivity();
        var class1 = new ActivityTraceFlagsEnricher();
        var @event = MakeLogEvent();
        class1.Enrich(@event, Mock.Of<ILogEventPropertyFactory>());

        var eventPropertiesNames = @event.Properties.Keys;
        var eventPropertiesValues = @event.Properties.Values;
        Assert.Collection(eventPropertiesNames, s => Assert.Equal("TraceFlags", s));
        Assert.Collection(eventPropertiesValues, s => Assert.Equal("None", s.ToString()));
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
