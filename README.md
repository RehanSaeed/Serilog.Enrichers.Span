![Banner](Images/Banner.png)

# Serilog.Enrichers.Span

[![GitHub Actions Status](https://github.com/RehanSaeed/Serilog.Enrichers.Span/workflows/Build/badge.svg?branch=main)](https://github.com/RehanSaeed/Serilog.Enrichers.Span/actions)

[![GitHub Actions Build History](https://buildstats.info/github/chart/RehanSaeed/Serilog.Enrichers.Span?branch=main&includeBuildsFromPullRequest=false)](https://github.com/RehanSaeed/Serilog.Enrichers.Span/actions)

Example showing how to setup an Azure Pipelines build status badge and build history bar chart:
```md
[![Azure Pipelines Overall Build Status](https://dev.azure.com/dotnet-boxed/Templates/_apis/build/status/Dotnet-Boxed.Templates?branchName=main)](https://dev.azure.com/dotnet-boxed/Templates/_build/latest?definitionId=2&branchName=main)

[![Azure Pipelines Build History](https://buildstats.info/azurepipelines/chart/dotnet-boxed/Templates/2?branch=main&includeBuildsFromPullRequest=false)](https://dev.azure.com/dotnet-boxed/Templates/_build/latest?definitionId=2&branchName=main)
```

Example showing how to setup an AppVeyor build status badge and build history bar chart:
```md
[![AppVeyor Build Status](https://ci.appveyor.com/api/projects/status/munmh9if4vfeqy62/branch/main?svg=true)](https://ci.appveyor.com/project/RehanSaeed/Serilog.Enrichers.Span/branch/main)

[![AppVeyor Build History](https://buildstats.info/appveyor/chart/RehanSaeed/Serilog.Enrichers.Span?branch=main&includeBuildsFromPullRequest=false)](https://ci.appveyor.com/project/RehanSaeed/Serilog.Enrichers.Span)
```

Enrich Serilog log events with properties from open telemetry spans.

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
