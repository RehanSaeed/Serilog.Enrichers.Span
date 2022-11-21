namespace Serilog.Enrichers.Span.Test;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Moq;
using Serilog.Core;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Parsing;
using Xunit;

public class ActivityEnricherTest
{
    private static readonly ActivitySource Source = new("Sample.DistributedTracing", "1.0.0");

    private static readonly ActivityListener Listener = new()
    {
        ShouldListenTo = _ => true,
        Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
    };

    public ActivityEnricherTest() => ActivitySource.AddActivityListener(Listener);

    [Fact]
    public void Given_no_configuration_When_no_custom_names_for_event_properties_Then_the_default_configuration_is_applied()
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
        var exception = Assert.Throws<ArgumentNullException>(() => new ActivityEnricher((null as SpanLogEventPropertiesNames)!));
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
    public void Given_config_with_no_TraceId_name_When_null_name_for_TraceId_event_property_Then_argument_exception_occurs()
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
    public void Given_config_with_no_SpanId_name_When_null_name_for_SpanId_event_property_Then_argument_exception_occurs()
    {
        using var act = Source.StartActivity();
        var exception = Assert.Throws<ArgumentException>(() => new SpanLogEventPropertiesNames
        {
            SpanId = null!,
        });
        Assert.Equal($"The property must not be empty (Parameter '{nameof(SpanLogEventPropertiesNames.SpanId)}')", exception.Message);
        Assert.Equal(nameof(SpanLogEventPropertiesNames.SpanId), exception.ParamName);
    }

    [Fact]
    public void Given_augmentor_When_augmentor_produces_properties_Then_additional_properties_are_added()
    {
        using var act = Source.StartActivity();

        var expectedSpanIdProperties = new[]
        {
            new LogEventProperty("alternate-span-id-name-1", new ScalarValue("alternate-span-id-1")),
            new LogEventProperty("alternate-span-id-name-2", new ScalarValue("alternate-span-id-2")),
        };

        var expectedTraceIdProperties = new[]
        {
            new LogEventProperty("alternate-trace-id-name-1", new ScalarValue("alternate-trace-id-1")),
        };

        var expectedParentIdProperties = Array.Empty<LogEventProperty>();

        var traceFormatConverter = new Mock<ISpanLogEventPropertyAugmentor>();
        traceFormatConverter.Setup(x => x.AugmentSpanId(It.IsAny<string>())).Returns(expectedSpanIdProperties);
        traceFormatConverter.Setup(x => x.AugmentTraceId(It.IsAny<string>())).Returns(expectedTraceIdProperties);
        traceFormatConverter.Setup(x => x.AugmentParentId(It.IsAny<string>())).Returns(expectedParentIdProperties);

        var class1 = new ActivityEnricher(traceFormatConverter.Object);

        var @event = MakeLogEvent();
        class1.Enrich(@event, Mock.Of<ILogEventPropertyFactory>());

        var expectedProperties = expectedSpanIdProperties
            .Concat(expectedTraceIdProperties)
            .Concat(expectedParentIdProperties);

        foreach (var expected in expectedProperties)
        {
            Assert.Contains(@event.Properties, entry => entry.Key == expected.Name && entry.Value == expected.Value);
        }
    }

    [Fact]
    public void Given_augmentor_When_additional_property_names_match_standard_names_Additional_properties_override_standard_properties()
    {
        using var act = Source.StartActivity();

        var propertyNames = new SpanLogEventPropertiesNames { ParentId = "p", SpanId = "s", TraceId = "t" };

        var expectedSpanIdProperties = new[]
        {
            new LogEventProperty(propertyNames.SpanId, new ScalarValue("alternate-span-id-1")),
        };

        var expectedTraceIdProperties = Array.Empty<LogEventProperty>();
        var expectedParentIdProperties = Array.Empty<LogEventProperty>();

        var traceFormatConverter = new Mock<ISpanLogEventPropertyAugmentor>();
        traceFormatConverter.Setup(x => x.AugmentSpanId(It.IsAny<string>())).Returns(expectedSpanIdProperties);
        traceFormatConverter.Setup(x => x.AugmentTraceId(It.IsAny<string>())).Returns(expectedTraceIdProperties);
        traceFormatConverter.Setup(x => x.AugmentParentId(It.IsAny<string>())).Returns(expectedParentIdProperties);

        var class1 = new ActivityEnricher(traceFormatConverter.Object);

        var @event = MakeLogEvent();
        class1.Enrich(@event, Mock.Of<ILogEventPropertyFactory>());

        Assert.Contains(@event.Properties, entry => entry.Key == propertyNames.SpanId && entry.Value == expectedSpanIdProperties[0].Value);
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
