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

public class ActivityOperationNameEnricherTest
{
    private static readonly ActivitySource Source = new("Sample.DistributedTracing", "1.0.0");

    private static readonly ActivityListener Listener = new()
    {
        ShouldListenTo = _ => true,
        Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
    };

    public ActivityOperationNameEnricherTest() => ActivitySource.AddActivityListener(Listener);

    [Fact]
    public void Given_no_configuration_When_no_custom_names_for_event_properties_Then_the_default_configuration_is_applied()
    {
        using var act = Source.StartActivity();
        var names = new SpanLogEventPropertiesNames();
        var class1 = new ActivityOperationNameEnricher(names);
        var @event = MakeLogEvent();
        class1.Enrich(@event, Mock.Of<ILogEventPropertyFactory>());

        var eventPropertiesNames = @event.Properties.Keys;
        Assert.Collection(eventPropertiesNames, s => Assert.Equal(names.OperationName, s));
    }

    [Fact]
    public void Given_config_with_names_When_custom_names_for_event_properties_Then_the_configuration_is_applied()
    {
        using var act = Source.StartActivity();
        var names = new SpanLogEventPropertiesNames { ParentId = "p", SpanId = "s", TraceId = "t", OperationName = "o" };
        var class1 =
            new ActivityOperationNameEnricher(names);

        var @event = MakeLogEvent();
        class1.Enrich(@event, Mock.Of<ILogEventPropertyFactory>());

        var eventPropertiesNames = @event.Properties.Keys;
        Assert.Collection(eventPropertiesNames, s => Assert.Equal(names.OperationName, s));
    }

    [Fact]
    public void Given_config_with_no_configuration_When_null_for_event_properties_Then_argument_exception_occurs()
    {
        using var act = Source.StartActivity();
        var exception = Assert.Throws<ArgumentNullException>(() => new ActivityOperationNameEnricher(null!));
        Assert.Equal("logEventPropertyNames", exception.ParamName);
    }

    [Fact]
    public void Given_config_with_no_OperationName_name_When_null_name_for_ParentId_event_property_Then_argument_exception_occurs()
    {
        using var act = Source.StartActivity();
        var exception = Assert.Throws<ArgumentException>(() => new SpanLogEventPropertiesNames
        {
            OperationName = null!,
        });
        Assert.Equal($"The property must not be empty (Parameter '{nameof(SpanLogEventPropertiesNames.OperationName)}')", exception.Message);
        Assert.Equal(nameof(SpanLogEventPropertiesNames.OperationName), exception.ParamName);
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
