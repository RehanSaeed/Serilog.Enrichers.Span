namespace Serilog.Enrichers.Span.Test;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;
using Moq;
using Serilog.Parsing;
using Serilog.Enrichers.Span;
using Xunit;

public class Class1Test
{
    private static readonly ActivitySource Source = new("Sample.DistributedTracing", "1.0.0");

    private static readonly ActivityListener Listener = new()
    {
        ShouldListenTo = _ => true,
        Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
    };

    public Class1Test() => ActivitySource.AddActivityListener(Listener);

    [Fact]
    public void Given_When_Then()
    {
        var class1 = new ActivityEnricher();

        Assert.NotNull(class1);
    }

    [Fact]
    public void
        Given_no_configuration_When_no_custom_names_for_event_properties_Then_the_default_configuration_is_applied()
    {
        using var act = Source.StartActivity();
        var class1 = new ActivityEnricher();
        var @event = MakeLogEvent();
        class1.Enrich(@event, Mock.Of<ILogEventPropertyFactory>());

        var eventPropertiesNames = @event.Properties.Keys;
        Assert.Collection(
            eventPropertiesNames,
            s => Assert.Equal("SpanId", s),
            s => Assert.Equal("TraceId", s),
            s => Assert.Equal("ParentId", s));
    }

    [Fact]
    public void Given_config_with_names_When_custom_names_for_event_properties_Then_the_configuration_is_applied()
    {
        using var act = Source.StartActivity();
        var class1 =
            new ActivityEnricher(new SpanLogEventPropertiesNames { ParentId = "p", SpanId = "s", TraceId = "t" });

        var @event = MakeLogEvent();
        class1.Enrich(@event, Mock.Of<ILogEventPropertyFactory>());

        var eventPropertiesNames = @event.Properties.Keys;
        Assert.Collection(
            eventPropertiesNames,
            s => Assert.Equal("s", s),
            s => Assert.Equal("t", s),
            s => Assert.Equal("p", s));
    }

    [Fact]
    public void Given_config_with_no_configuration_When_null_for_event_properties_Then_argument_exception_occurs()
    {
        using var act = Source.StartActivity();
        var exception = Assert.Throws<ArgumentNullException>(() => new ActivityEnricher(null!));
        Assert.Equal("logEventPropertyNames", exception.ParamName);
    }

    [Fact]
    public void Given_config_with_no_ParentId_name_When_null_name_for_ParentId_event_property_Then_argument_exception_occurs()
    {
        using var act = Source.StartActivity();
        var exception = Assert.Throws<ArgumentException>(() => new SpanLogEventPropertiesNames
        {
            ParentId = null!,
        });
        Assert.Equal($"The property must not be empty (Parameter '{nameof(SpanLogEventPropertiesNames.ParentId)}')", exception.Message);
        Assert.Equal(nameof(SpanLogEventPropertiesNames.ParentId), exception.ParamName);
    }

    [Fact]
    public void Given_config_with_no_ParentId_name_When_null_name_for_TraceId_event_property_Then_argument_exception_occurs()
    {
        using var act = Source.StartActivity();
        var exception = Assert.Throws<ArgumentException>(() => new SpanLogEventPropertiesNames
        {
            TraceId = null!,
        });
        Assert.Equal($"The property must not be empty (Parameter '{nameof(SpanLogEventPropertiesNames.TraceId)}')", exception.Message);
        Assert.Equal(nameof(SpanLogEventPropertiesNames.TraceId), exception.ParamName);
    }

    [Fact]
    public void Given_config_with_no_ParentId_name_When_null_name_for_SpanId_event_property_Then_argument_exception_occurs()
    {
        using var act = Source.StartActivity();
        var exception = Assert.Throws<ArgumentException>(() => new SpanLogEventPropertiesNames
        {
            SpanId = null!,
        });
        Assert.Equal($"The property must not be empty (Parameter '{nameof(SpanLogEventPropertiesNames.SpanId)}')", exception.Message);
        Assert.Equal(nameof(SpanLogEventPropertiesNames.SpanId), exception.ParamName);
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
