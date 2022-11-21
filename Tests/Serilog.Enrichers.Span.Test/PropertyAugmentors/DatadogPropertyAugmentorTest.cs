namespace Serilog.Enrichers.Span.Test.TraceFormatConverters;

using System.Linq;
using Serilog.Enrichers.Span.TraceFormatConverters;
using Serilog.Events;
using Xunit;

public class DatadogPropertyAugmentorTest
{
    [Fact]
    public void Converting_TraceId_When_input_is_16_byte_hex_string_Rightmost_8_bytes_are_converted_to_uint64()
    {
        var class1 = new DatadogPropertyAugmentor();
        var additionalProperties = class1.AugmentTraceId("ebe1010db7d9a5dec2b4a9915c2c79f0").ToList();
        Assert.Single(additionalProperties);
        var property = additionalProperties[0];
        Assert.Equal(new ScalarValue("14030025180947708400"), property.Value);
    }

    [Fact]
    public void Converting_TraceId_When_no_property_name_is_given_Property_has_default_name()
    {
        var class1 = new DatadogPropertyAugmentor();
        var additionalProperties = class1.AugmentTraceId("ebe1010db7d9a5dec2b4a9915c2c79f0").ToList();
        Assert.Single(additionalProperties);
        var property = additionalProperties[0];
        Assert.Equal(DatadogPropertyAugmentor.DefaultTraceIdName, property.Name);
    }

    [Fact]
    public void Converting_TraceId_When_property_name_is_given_Property_has_specified_name()
    {
        var expectedTraceIdName = "expected-trace-id-name";
        var class1 = new DatadogPropertyAugmentor(traceIdName: expectedTraceIdName);
        var additionalProperties = class1.AugmentTraceId("ebe1010db7d9a5dec2b4a9915c2c79f0").ToList();
        Assert.Single(additionalProperties);
        var property = additionalProperties[0];
        Assert.Equal(expectedTraceIdName, property.Name);
    }

    [Fact]
    public void Converting_TraceId_When_input_is_not_a_16_byte_hex_string_No_property_generated()
    {
        var class1 = new DatadogPropertyAugmentor();
        var additionalProperties = class1.AugmentTraceId("some-trace-id");
        Assert.Empty(additionalProperties);
    }

    [Fact]
    public void Converting_SpanId_When_input_is_8_byte_hex_string_Value_is_converted_to_uint64()
    {
        var class1 = new DatadogPropertyAugmentor();
        var additionalProperties = class1.AugmentSpanId("69e9851f1ca02b9a").ToList();
        Assert.Single(additionalProperties);
        var property = additionalProperties[0];
        Assert.Equal(new ScalarValue("7631777412226755482"), property.Value);
    }

    [Fact]
    public void Converting_SpanId_When_no_property_name_is_given_Property_has_default_name()
    {
        var class1 = new DatadogPropertyAugmentor();
        var additionalProperties = class1.AugmentSpanId("69e9851f1ca02b9a").ToList();
        Assert.Single(additionalProperties);
        var property = additionalProperties[0];
        Assert.Equal(DatadogPropertyAugmentor.DefaultSpanIdName, property.Name);
    }

    [Fact]
    public void Converting_SpanId_When_property_name_is_given_Property_has_specified_name()
    {
        var expectedSpanIdName = "expected-span-id-name";
        var class1 = new DatadogPropertyAugmentor(spanIdName: expectedSpanIdName);
        var additionalProperties = class1.AugmentSpanId("69e9851f1ca02b9a").ToList();
        Assert.Single(additionalProperties);
        var property = additionalProperties[0];
        Assert.Equal(expectedSpanIdName, property.Name);
    }

    [Fact]
    public void Converting_SpanId_Input_is_not_a_8_byte_hex_string_No_property_generated()
    {
        var class1 = new DatadogPropertyAugmentor();
        var additionalProperties = class1.AugmentSpanId("some-span-id");
        Assert.Empty(additionalProperties);
    }

    [Fact]
    public void Converting_ParentId_No_property_generated()
    {
        var class1 = new DatadogPropertyAugmentor();
        var converted = class1.AugmentParentId("69e9851f1ca02b9a");
        Assert.Empty(converted);
    }
}
