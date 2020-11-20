![Banner](Images/Banner.png)

[![NuGet Package](https://img.shields.io/nuget/v/Serilog.Enrichers.Span.svg)](https://www.nuget.org/packages/Serilog.Enrichers.Span/) [![Serilog.Enrichers.Span package in serilog-exceptions feed in Azure Artifacts](https://feeds.dev.azure.com/serilog-exceptions/_apis/public/Packaging/Feeds/8479813c-da6b-4677-b40d-78df8725dc9c/Packages/212043f6-5fe5-4c79-949e-162156b89894/Badge)](https://dev.azure.com/serilog-exceptions/Serilog.Exceptions/_packaging?_a=package&feed=8479813c-da6b-4677-b40d-78df8725dc9c&package=212043f6-5fe5-4c79-949e-162156b89894&preferRelease=true) [![Twitter URL](https://img.shields.io/twitter/url/http/shields.io.svg?style=social)](https://twitter.com/RehanSaeedUK) [![Twitter Follow](https://img.shields.io/twitter/follow/rehansaeeduk.svg?style=social&label=Follow)](https://twitter.com/RehanSaeedUK)

Enrich Serilog log events with properties from open telemetry spans using .NET's [Activity](https://docs.microsoft.com/dotnet/api/system.diagnostics.activity) API.

# What Does It Do?










# Getting Started

Add the [Serilog.Enrichers.Span](https://www.nuget.org/packages/Serilog.Enrichers.Span/) Serilog.Exceptions is an add-on to [Serilog](https://serilog.net) to log exception details and custom properties that are not output in `Exception.ToString()`.



```cs
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogging(loggingBuilder => loggingBuilder.Configure(options => options.ActivityTrackingOptions = ActivityTrackingOptions.TraceId | ActivityTrackingOptions.SpanId))
        .UseSerilog(
            (context, loggerConfiguration) =>
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithSpan()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console(new RenderedCompactJsonFormatter()))
        .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
```





[![GitHub Actions Status](https://github.com/RehanSaeed/Serilog.Enrichers.Span/workflows/Build/badge.svg?branch=main)](https://github.com/RehanSaeed/Serilog.Enrichers.Span/actions)

[![GitHub Actions Build History](https://buildstats.info/github/chart/RehanSaeed/Serilog.Enrichers.Span?branch=main&includeBuildsFromPullRequest=false)](https://github.com/RehanSaeed/Serilog.Enrichers.Span/actions)
