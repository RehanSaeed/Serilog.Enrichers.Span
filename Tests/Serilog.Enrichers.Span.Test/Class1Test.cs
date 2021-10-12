namespace Serilog.Enrichers.Span.Test;

using Serilog.Enrichers.Span;
using Xunit;

public class Class1Test
{
    [Fact]
    public void Given_When_Then()
    {
        var class1 = new ActivityEnricher();

        Assert.NotNull(class1);
    }
}
