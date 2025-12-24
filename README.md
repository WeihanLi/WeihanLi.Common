# WeihanLi.Common

[![WeihanLi.Common Latest Stable](https://img.shields.io/nuget/v/WeihanLi.Common.svg)](https://www.nuget.org/packages/WeihanLi.Common/)
[![WeihanLi.Common Latest Preview](https://img.shields.io/nuget/vpre/WeihanLi.Common)](https://www.nuget.org/packages/WeihanLi.Common/absoluteLatest)
[![Azure Pipelines Build Status](https://weihanli.visualstudio.com/Pipelines/_apis/build/status/WeihanLi.WeihanLi.Common?branchName=master)](https://weihanli.visualstudio.com/Pipelines/_build/latest?definitionId=16&branchName=master)
[![GitHub Actions Build Status](https://github.com/WeihanLi/WeihanLi.Common/actions/workflows/default.yml/badge.svg)](https://github.com/WeihanLi/WeihanLi.Common/actions/workflows/default.yml)

## Overview

`WeihanLi.Common` bundles a set of production-ready helpers, extensions, and middleware to simplify .NET application development. It covers dependency injection, AOP, eventing, logging, data access helpers, OTP utilities, templating, and more—built from real-world usage and battle-tested across services.

## Packages

| Package | Description |
| --- | --- |
| `WeihanLi.Common` | Core helpers and extensions: dependency resolver, configuration helpers, pipelines, TOTP utilities, command executors, etc. |
| `WeihanLi.Common.Logging.Serilog` | Integration layer so core logging abstractions seamlessly forward to Serilog or Microsoft.Extensions.Logging. |
| `WeihanLi.Extensions.Hosting` | Hosting helpers for quickly wiring background services, console apps, and DI bootstrapping. |

Each package is available on NuGet and can be referenced independently.

## Installation

Install the package you need using the .NET CLI or NuGet Package Manager:

```bash
dotnet add package WeihanLi.Common
# or install the Serilog integration
dotnet add package WeihanLi.Common.Logging.Serilog
```

Preview builds are also published; append `--version <preview>` to opt in.

## Quick Start

```csharp
using WeihanLi.Common.Helpers;

// Generate a TOTP code with the built-in helper.
var secret = $"{ApplicationHelper.ApplicationName}_demo_secret";
var code = TotpHelper.GenerateCode(secret);

Console.WriteLine($"Generated code: {code}");

var isValid = TotpHelper.ValidateCode(secret, code);
Console.WriteLine($"Valid code: {isValid}");
```

See `samples/DotNetCoreSample` and `samples/AspNetCoreSample` for richer scenarios that exercise DI decorators, event buses, logging pipelines, and more.

## Feature Highlights

- **Dependency Injection**: Lightweight container abstractions, decorator helpers, metadata-based registrations, proxy support, and integration with `Microsoft.Extensions.DependencyInjection`.
- **Fluent Aspects (AOP)**: Dynamic proxy-based interception with fluent configuration for method-level behaviors (logging, validation, caching, etc.).
- **Event Infrastructure**: In-memory and queue-backed `EventBus`, `EventStore`, and handler helpers to wire publish/subscribe workflows.
- **Logging**: Abstractions with adapters for built-in logging and Serilog plus formatting helpers to ship structured events.
- **Data Access Helpers**: Dapper-style extensions over ADO.NET, entity mapping utilities, and SQL expression parsers to streamline data access layers.
- **OTP & Security**: TOTP implementation, helpers, and service abstractions to secure user flows with minimal setup.
- **Templating & Text**: Lightweight template engine and string utility helpers for dynamic content rendering.
- **Utilities**: Process executor, pipeline builder, command helpers, configuration helpers, async invokers, and more.

## Documentation & Samples

- Browse the docs in `docs/index.md` and articles under `docs/articles` for deep dives.
- API references can be generated with DocFX (`docs/docfx.json`).
- Samples live under `samples/` and demonstrate step-by-step usage patterns you can adapt.

## Building & Testing

```bash
dotnet build
dotnet test
```

CI builds run with both Azure Pipelines (`azure-pipelines.yml`) and GitHub Actions (`.github/workflows/default.yml`), so you can rely on the same commands locally.

## Release Notes

Head to the [merged pull requests list](https://github.com/WeihanLi/WeihanLi.Common/pulls?q=is%3Apr+is%3Amerged+base%3Amaster) or the docfx [ReleaseNotes](docs/ReleaseNotes.md) for a chronological view of changes.
