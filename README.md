![Banner](https://github.com/RehanSaeed/Serilog.Enrichers.Span/blob/main/Images/Banner.png)

[![Serilog.Enrichers.Span NuGet Package](https://img.shields.io/nuget/v/Serilog.Enrichers.Span.svg)](https://www.nuget.org/packages/Serilog.Enrichers.Span/) [![Serilog.Enrichers.Span package in serilog-exceptions feed in Azure Artifacts](https://feeds.dev.azure.com/serilog-exceptions/_apis/public/Packaging/Feeds/8479813c-da6b-4677-b40d-78df8725dc9c/Packages/3f8a2a7e-8124-4987-9c44-916dba83b9d6/Badge)](https://dev.azure.com/serilog-exceptions/Serilog.Enrichers.Span/_packaging?_a=package&feed=8479813c-da6b-4677-b40d-78df8725dc9c&package=3f8a2a7e-8124-4987-9c44-916dba83b9d6&preferRelease=true) [![Serilog.Enrichers.Span NuGet Package Downloads](https://img.shields.io/nuget/dt/Serilog.Enrichers.Span)](https://www.nuget.org/packages/Serilog.Enrichers.Span) [![Twitter URL](https://img.shields.io/twitter/url/http/shields.io.svg?style=social)](https://twitter.com/RehanSaeedUK) [![Twitter Follow](https://img.shields.io/twitter/follow/rehansaeeduk.svg?style=social&label=Follow)](https://twitter.com/RehanSaeedUK)

Enrich Serilog log events with properties from open telemetry spans using .NET's [Activity](https://docs.microsoft.com/dotnet/api/system.diagnostics.activity) API.

# What Does It Do?

Enriches logs with a spans unique identifier, parent unique identifier and ASP.NET's trace unique identifier. `Serilog.Enrichers.Span` is an add-on to [Serilog](https://serilog.net) to log exception details and custom properties that are not output in `Exception.ToString()`.

# Getting Started

Add the [Serilog.Enrichers.Span](https://www.nuget.org/packages/Serilog.Enrichers.Span/) NuGet package.

```powershell
dotnet add package Serilog.Enrichers.Span
```

When setting up your logger, add the `WithSpan()` line like so:

```cs
using Serilog;
using Serilog.Enrichers.Span;

ILogger logger = new LoggerConfiguration()
    .Enrich.WithSpan()
    .WriteTo.RollingFile(
        new JsonFormatter(renderMessage: true), 
        @"C:\logs\log-{Date}.txt")    
    .CreateLogger();
```

## Options

Program.cs

```cs
    .Enrich.WithSpan(new SpanOptions {
        IncludeTags = true,
        IncludeBaggage = true,
        IncludeOperationName = true,
        IncludeTraceFlags = true,
        IncludeTraceState = true })
```

appsettings.json (requires Serilog.Settings.Configuration)

```json
{
    "Serilog": {
        "Enrich": [{
            "Name": "WithSpan",
            "Args": {
                "SpanOptions": {
                    "IncludeTags": true,
                    "IncludeBaggage": true,
                    "IncludeOperationName": true,
                    "IncludeTraceFlags": true,
                    "IncludeTraceState": true
                }
            }
        }]
    }
}
```

| Option | Description |
| --- | --- |
| IncludeTags | Include [Activity.Tags](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.activity.tags) as name `Attributes` when enriching log entry if true. Default false. |
| IncludeBaggage | Include [Activity.Baggage](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.activity.baggage) as name `Baggage` when enriching log entry if true. Default false. |
| IncludeOperationName | Include [Activity.OperationName](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.activity.operationname) as name equal to it's value when enriching log entry if true. Default false.  |
| IncludeTraceFlags | Include [Activity.ActivityTraceFlags](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.activity.activitytraceflags) as name `TraceFlags` when enriching log entry if true. Default false. |
| IncludeTraceState | Include [Activity.TraceStateString](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.activity.tracestatestring) as name `TraceState` when enriching log entry if true. Default false. |

## Continuous Integration

| Name            | Operating System      | Status | History |
| :---            | :---                  | :---   | :---    |
| Azure Pipelines | Ubuntu                | [![Azure Pipelines Ubuntu Build Status](https://dev.azure.com/serilog-exceptions/Serilog.Enrichers.Span/_apis/build/status/RehanSaeed.Serilog.Enrichers.Span?branchName=main&stageName=Build&jobName=Build&configuration=Build%20Linux)](https://dev.azure.com/serilog-exceptions/Serilog.Enrichers.Span/_build/latest?definitionId=2&branchName=main) |
| Azure Pipelines | Mac                   | [![Azure Pipelines Mac Build Status](https://dev.azure.com/serilog-exceptions/Serilog.Enrichers.Span/_apis/build/status/RehanSaeed.Serilog.Enrichers.Span?branchName=main&stageName=Build&jobName=Build&configuration=Build%20Mac)](https://dev.azure.com/serilog-exceptions/Serilog.Enrichers.Span/_build/latest?definitionId=2&branchName=main) |
| Azure Pipelines | Windows               | [![Azure Pipelines Windows Build Status](https://dev.azure.com/serilog-exceptions/Serilog.Enrichers.Span/_apis/build/status/RehanSaeed.Serilog.Enrichers.Span?branchName=main&stageName=Build&jobName=Build&configuration=Build%20Windows)](https://dev.azure.com/serilog-exceptions/Serilog.Enrichers.Span/_build/latest?definitionId=2&branchName=main) |
| Azure Pipelines | Overall               | [![Azure Pipelines Overall Build Status](https://dev.azure.com/serilog-exceptions/Serilog.Enrichers.Span/_apis/build/status/RehanSaeed.Serilog.Enrichers.Span?branchName=main)](https://dev.azure.com/serilog-exceptions/Serilog.Enrichers.Span/_build/latest?definitionId=2&branchName=main) | [![Azure DevOps Build History](https://buildstats.info/azurepipelines/chart/serilog-exceptions/Serilog.Enrichers.Span/2?branch=main&includeBuildsFromPullRequest=false)](https://dev.azure.com/serilog-exceptions/Serilog.Enrichers.Span/_build/latest?definitionId=2&branchName=main) |
| GitHub Actions  | Ubuntu, Mac & Windows | [![GitHub Actions Status](https://github.com/RehanSaeed/Serilog.Enrichers.Span/workflows/Build/badge.svg?branch=main)](https://github.com/RehanSaeed/Serilog.Enrichers.Span/actions) | [![GitHub Actions Build History](https://buildstats.info/github/chart/RehanSaeed/Serilog.Enrichers.Span?branch=main&includeBuildsFromPullRequest=false)](https://github.com/RehanSaeed/Serilog.Enrichers.Span/actions) |
| AppVeyor        | Ubuntu, Mac & Windows | [![AppVeyor Build status](https://ci.appveyor.com/api/projects/status/gd9pt19ekymmr79g?svg=true)](https://ci.appveyor.com/project/RehanSaeed/serilog-enrichers-span) | [![AppVeyor Build History](https://buildstats.info/appveyor/chart/RehanSaeed/serilog-enrichers-span?branch=main&includeBuildsFromPullRequest=false)](https://ci.appveyor.com/project/RehanSaeed/serilog-enrichers-span) |

## Contributions and Thanks

Please view the [contributing guide](https://github.com/RehanSaeed/Serilog.Enrichers.Span/blob/main/.github/CONTRIBUTING.md) for more information.
