namespace Serilog.Enrichers.Span.Test
{
    using System.Diagnostics;
    using Serilog.Enrichers.Span;
    using Serilog.Events;
    using Xunit;

    public class Class1Test
    {
        [Fact]
        public void Given_When_Then()
        {
            var class1 = new ActivityEnricher();

            Assert.NotNull(class1);
        }

        [Fact]
        public void AddTags()
        {
            LogEvent? evt = null;
            var log = new LoggerConfiguration()
                .Enrich.WithSpan()
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            ActivitySource.AddActivityListener(new ActivityListener
            {
                ActivityStarted = activity => { },
                ActivityStopped = activity => { },
                ShouldListenTo = activitySource => true,
                SampleUsingParentId = (ref ActivityCreationOptions<string> activityOptions) => ActivitySamplingResult.AllDataAndRecorded,
                Sample = (ref ActivityCreationOptions<ActivityContext> activityOptions) => ActivitySamplingResult.AllDataAndRecorded,
            });
            var activitySource = new ActivitySource("Test");

            
            using (var activity = activitySource.StartActivity("AddTags"))
            {
                activity?.AddTag("Name", "Chris Shim");
                activity?.AddTag("NullString", null);

                log.Information(@"Has an EnvironmentUserName property with [domain\]userName");
            }

            Assert.NotNull(evt);

            Assert.Equal("Chris Shim", evt?.Properties["Name"].LiteralValue());
            Assert.Null(evt?.Properties["NullString"].LiteralValue());
        }
    }

    internal static class Extensions
    {
        public static object LiteralValue(this LogEventPropertyValue @this)
        {
            return ((ScalarValue)@this).Value;
        }
    }
}
