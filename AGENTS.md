# AGENTS.md - WeihanLi.Common

This file gives AI coding agents the repository-specific context needed to make correct, focused changes. It applies to the whole repository unless a nested `AGENTS.md` appears in a subdirectory.

## Project Overview

`WeihanLi.Common` is a production-ready .NET utility library that bundles helpers, extensions, middleware, and application primitives for .NET development. It covers AOT-friendly core utilities, dependency injection, Fluent Aspects AOP, eventing, logging, data access helpers, OTP utilities, templating, hosting helpers, and more.

Four NuGet packages are published from this repository:

| Package | Description |
|---|---|
| `WeihanLi.Core` | AOT-compatible core extensions, helpers, models, compression primitives, OTP utilities, and dependency-light APIs |
| `WeihanLi.Common` | Full utility package built on `WeihanLi.Core`, including DI, AOP, logging, data access, HTTP, JSON, configuration, templating, and eventing |
| `WeihanLi.Common.Logging.Serilog` | Serilog integration layer for `WeihanLi.Common.Logging` |
| `WeihanLi.Extensions.Hosting` | Hosting helpers for background services, console apps, and DI bootstrapping |

## Repository Map

```text
src/
  WeihanLi.Core/                    AOT-friendly core package; root namespace remains WeihanLi.Common
  WeihanLi.Common/                  Full utility package referencing WeihanLi.Core
  WeihanLi.Common.Logging.Serilog/  Serilog integration package
  WeihanLi.Extensions.Hosting/      Hosting extensions package
test/
  WeihanLi.Common.Test/             xUnit v3 tests on Microsoft Testing Platform
samples/
  AspNetCoreSample/                 ASP.NET Core examples
  DotNetCoreSample/                 Console examples
perf/
  WeihanLi.Common.Benchmark/        BenchmarkDotNet benchmarks
docs/                               DocFX documentation
build/                              Versioning and signing MSBuild props
```

Important root files:

| File | Purpose |
|---|---|
| `WeihanLi.Common.slnx` | Solution file |
| `build.cs` | C# build script used by CI |
| `Directory.Build.props` | Shared MSBuild properties; sets `LatestTargetFramework` to `net10.0` |
| `src/Directory.Build.props` | Source-package MSBuild properties, docs, packaging, signing, Source Link |
| `Directory.Packages.props` | Central package management and NuGet audit configuration |
| `.github/workflows/default.yml` | GitHub Actions CI |
| `azure-pipelines.yml` | Azure Pipelines CI |

There is currently no `global.json`; use the SDK versions required by the project and CI instead of assuming an SDK pin.

## Build And Test

Prerequisites:

- Install .NET SDK `10.0.x` and .NET SDK `11.0.x` preview when building all source projects, because `WeihanLi.Core` and `WeihanLi.Common` target `net11.0`.
- CI installs both SDKs. Azure Pipelines uses `includePreviewVersions: true` for .NET 11.

Common commands:

```bash
# Build the solution
dotnet build

# Run all tests
dotnet test

# Full CI build: build, test, and pack according to build.cs
dotnet build.cs

# Verify formatting
dotnet format --verify-no-changes

# Apply formatting
dotnet format
```

Set `DISABLE_GITHUB_ACTIONS_TEST_LOGGER=true` to opt out of the GitHub Actions test logger when running tests locally.

Before submitting a change, run the narrowest relevant test first, then run these required checks when practical:

```bash
dotnet build
dotnet test
dotnet format --verify-no-changes
```

## Target Frameworks

| Project | Target frameworks |
|---|---|
| `src/WeihanLi.Core` | `netstandard2.0`, `net8.0`, `net10.0`, `net11.0` |
| `src/WeihanLi.Common` | `netstandard2.0`, `net8.0`, `net10.0`, `net11.0` |
| `src/WeihanLi.Common.Logging.Serilog` | `netstandard2.0`, `net10.0` |
| `src/WeihanLi.Extensions.Hosting` | `net10.0` |
| `test/WeihanLi.Common.Test` | `$(LatestTargetFramework)` (`net10.0`) |

Use conditional compilation for framework-specific behavior, for example `#if NET8_0_OR_GREATER`, `#if NET10_0_OR_GREATER`, or explicit `netstandard2.0` conditions. Be careful with `Reflection.Emit` and APIs missing from `netstandard2.0`.

## Key Namespaces And Components

| Namespace | Purpose |
|---|---|
| `WeihanLi.Common` | Core utilities such as `Guard`, `CacheUtil`, and `DependencyResolver` |
| `WeihanLi.Common.Abstractions` | Shared primitives and property bags |
| `WeihanLi.Common.Aspect` | Fluent Aspects AOP, dynamic proxies, interceptors, and invocation pipeline |
| `WeihanLi.Common.Compressor` | Compression helpers from `WeihanLi.Core` |
| `WeihanLi.Common.Data` | ADO.NET extensions, repository helpers, entity mapping, SQL expression parsers |
| `WeihanLi.Common.DependencyInjection` | Lightweight DI container, service definitions, modules, decorators |
| `WeihanLi.Common.Event` | `EventBus`, `EventQueue`, `EventStore`, publish/subscribe helpers |
| `WeihanLi.Common.Helpers` | `ApplicationHelper`, `TotpHelper`, `CommandExecutor`, `ConsoleHelper`, cron and pipeline helpers |
| `WeihanLi.Common.Http` | HTTP client utilities and mock handlers |
| `WeihanLi.Common.Json` | JSON converters and serialization helpers |
| `WeihanLi.Common.Logging` | Logging abstractions and adapters |
| `WeihanLi.Common.Models` | Result, paging, entity, tenant, validation, and common model types |
| `WeihanLi.Common.Otp` | TOTP/OTP implementation |
| `WeihanLi.Common.Services` | Common service abstractions and implementations |
| `WeihanLi.Common.Template` | Lightweight template engine |
| `WeihanLi.Extensions` | Shared extension methods |

## Code Style

- C# nullable reference types and implicit usings are enabled.
- `LangVersion` is `preview`; do not use preview-only syntax casually when older target frameworks need compatibility.
- Public APIs use PascalCase. Private fields use `_camelCase`; private static readonly fields may use PascalCase when matching the existing style.
- Source files should include the Apache License 2.0 header used throughout the repository.
- XML documentation is required for public APIs in source packages.
- Prefer existing helpers, extension patterns, and namespace layout over introducing new abstractions.
- Keep changes narrowly scoped; avoid unrelated refactors or generated-file churn.
- Package versions are centrally managed in `Directory.Packages.props`; do not add `<Version>` metadata in individual `.csproj` files.
- The repository has T4-generated files such as `ServiceContainerBuilderExtensions.generated.cs`, `DbCommandExtension.generated.cs`, and `DbConnectionExtension.generated.cs`; update the `.tt` source when changing generated APIs.

### Parameter Validation

Use `Guard` utilities for parameter validation:

```csharp
public static string Process(string input)
{
    Guard.NotNullOrEmpty(input);
    return input;
}
```

### Extension Methods

Place extension methods in dedicated files under the appropriate `Extensions/` folder and use the `WeihanLi.Extensions` namespace:

```csharp
namespace WeihanLi.Extensions;

public static class StringExtension
{
    public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);
}
```

### Fluent APIs

Many components expose fluent configuration APIs. Match the existing style:

```csharp
FluentAspects.Configure(options =>
    options.InterceptAll()
           .With<LoggingInterceptor>()
);
```

## Testing Guidelines

- Tests live under `test/WeihanLi.Common.Test/`.
- The test stack is xUnit v3 on Microsoft Testing Platform.
- Test files are named `{ComponentName}Test.cs`.
- Test namespace is usually `WeihanLi.Common.Test` or a component-specific child namespace.
- Test method names should follow `MethodName_Scenario_ExpectedResult`.
- Use `[Theory]` and `[InlineData]` for parameterized coverage.
- Add or update tests for behavior changes, bug fixes, and public API additions.

Example:

```csharp
[Fact]
public void MethodName_Scenario_ExpectedResult()
{
    // Arrange
    var input = "test";

    // Act
    var result = SystemUnderTest.Process(input);

    // Assert
    Assert.NotNull(result);
}
```

## Common Development Tasks

### Add A Core Utility Or Extension

1. Prefer `src/WeihanLi.Core/` when the API is dependency-light and AOT-compatible.
2. Use existing namespaces such as `WeihanLi.Common.Helpers`, `WeihanLi.Common.Models`, or `WeihanLi.Extensions`.
3. Preserve compatibility with all target frameworks.
4. Add XML documentation and focused tests.

### Add A Full Common Feature

1. Use `src/WeihanLi.Common/` for features that depend on DI, logging, data access, HTTP, JSON, configuration, AOP, or other heavier integrations.
2. Define abstractions when the existing design calls for them.
3. Register services with the existing DI patterns when applicable.
4. Add options classes for configurable behavior.
5. Add tests and update samples for significant new behavior.

### Work With Fluent Aspects

```csharp
FluentAspects.Configure(options =>
{
    options.InterceptMethod<IService>(s => s.Process(Argument.Any<string>()))
           .With<ValidationInterceptor>();
});

var service = FluentAspects.AspectOptions.ProxyFactory
    .CreateProxy<IService>(new ServiceImplementation());
```

### Work With Data Access

```csharp
var users = connection.Select<User>(
    "SELECT * FROM Users WHERE Age > @age", new { age = 18 });

var repository = new Repository<User>(() => connectionFactory.GetConnection());
var user = repository.Fetch(u => u.Id == userId);
```

## Security

- Validate inputs with `Guard`.
- Dispose resources properly with `using` declarations or equivalent ownership patterns.
- Use `RandomNumberGenerator` instead of `Random` for cryptographic or security-sensitive randomness.
- Never commit secrets, credentials, tokens, or local machine configuration.
- NuGet audit is enabled and NU1901-NU1904 are warnings-as-errors through central package management; do not suppress audit warnings without justification.

## Documentation

- Update XML docs for public API changes.
- Update package README files under `src/*/README.md` when package-level behavior changes.
- Docs live under `docs/` and are generated with DocFX (`docs/docfx.json`).
- Release notes live in `docs/ReleaseNotes.md`.
- Samples in `samples/` should demonstrate significant end-to-end usage patterns.

## Pull Requests

Use Conventional Commits for commit messages and PR titles:

```text
<type>[optional scope]: <description>
```

Common types: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`.

Examples:

```text
feat(event): add retry support to EventBus
fix(otp): correct TOTP window boundary calculation
docs: update AGENTS.md with current project guidance
build: bump Newtonsoft.Json to 13.0.4
feat!: remove obsolete IDependencyResolver overloads
```

For breaking changes, append `!` after the type/scope or add a `BREAKING CHANGE:` footer.

## Troubleshooting

- Build failures: run `dotnet build --verbosity detailed`.
- Test failures on the current test TFM: run `dotnet test test/WeihanLi.Common.Test/WeihanLi.Common.Test.csproj`.
- NuGet restore errors: check `Directory.Packages.props` first.
- Missing API on `netstandard2.0`: add framework guards or use compatible alternatives.
- Local GitHub Actions logger issues: set `DISABLE_GITHUB_ACTIONS_TEST_LOGGER=true`.
