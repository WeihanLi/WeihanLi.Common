# AGENTS.md — WeihanLi.Common

This file gives AI coding agents the context needed to work on this repository effectively.

## Project Overview

**WeihanLi.Common** is a production-ready .NET utility library that bundles helpers, extensions, and middleware for .NET application development. It covers dependency injection, AOP (Fluent Aspects), eventing, logging, data access helpers, OTP utilities, templating, and more.

Three NuGet packages are published from this repository:

| Package | Description |
|---|---|
| `WeihanLi.Common` | Core helpers, extensions, DI, AOP, event bus, TOTP, templating, and more |
| `WeihanLi.Common.Logging.Serilog` | Integration layer forwarding to Serilog or Microsoft.Extensions.Logging |
| `WeihanLi.Extensions.Hosting` | Helpers for background services, console apps, and DI bootstrapping |

## Repository Structure

```
├── src/
│   ├── WeihanLi.Common/                    # Core library
│   ├── WeihanLi.Common.Logging.Serilog/    # Serilog integration
│   └── WeihanLi.Extensions.Hosting/        # Hosting extensions
├── test/
│   └── WeihanLi.Common.Test/               # xUnit tests
├── samples/
│   ├── AspNetCoreSample/                   # ASP.NET Core usage examples
│   └── DotNetCoreSample/                   # Console app examples
├── perf/
│   └── WeihanLi.Common.Benchmark/         # BenchmarkDotNet benchmarks
├── docs/                                   # DocFX documentation
├── build/                                  # Build configuration (signing, versioning)
├── .github/workflows/                      # GitHub Actions CI/CD
├── build.cs                                # C# build script (run with dotnet build.cs)
├── WeihanLi.Common.slnx                   # Solution file (modern .slnx format)
├── Directory.Build.props                   # Centralized MSBuild configuration
├── Directory.Packages.props               # Centralized NuGet package versions
└── global.json                            # .NET SDK version pin
```

## Build and Test

### Prerequisites

- .NET SDK 10.0+ (see `global.json`; `rollForward` is enabled for newer versions)
- Supported SDK versions for CI: 8.0.x, 9.0.x, 10.0.x

### Commands

```bash
# Build the entire solution
dotnet build

# Run all tests
dotnet test

# Full CI build (build + test + pack)
dotnet build.cs

# Format code
dotnet format
```

> Set `DISABLE_GITHUB_ACTIONS_TEST_LOGGER=true` to opt out of the GitHub Actions test logger when running tests locally.

### CI

- **GitHub Actions** (`.github/workflows/default.yml`): runs `dotnet build.cs` on Ubuntu, macOS, and Windows
- **Azure Pipelines** (`azure-pipelines.yml`): additional pipeline for Azure DevOps

## Target Frameworks

- `WeihanLi.Common`: `netstandard2.0`, `net8.0`, `net9.0`, `net10.0`
- `WeihanLi.Common.Logging.Serilog`: `netstandard2.0`, `net8.0`
- `WeihanLi.Extensions.Hosting`: `net8.0`

Use conditional compilation (`#if NET8_0_OR_GREATER` etc.) for framework-specific code.

## Key Namespaces and Components

| Namespace | Purpose |
|---|---|
| `WeihanLi.Common` | Core utilities, `Guard`, `CacheUtil`, `DependencyResolver` |
| `WeihanLi.Common.Aspect` | Fluent Aspects AOP — dynamic proxies, interceptors, invocation |
| `WeihanLi.Common.Data` | ADO.NET extensions, entity mapping, SQL expression parsers |
| `WeihanLi.Common.DependencyInjection` | Lightweight DI container, service definitions, module support |
| `WeihanLi.Common.Event` | `EventBus`, `EventQueue`, `EventStore`, publish/subscribe |
| `WeihanLi.Common.Extensions` | Extension methods for core .NET types |
| `WeihanLi.Common.Helpers` | `ApplicationHelper`, `TotpHelper`, `CommandExecutor`, `ConsoleHelper`, etc. |
| `WeihanLi.Common.Http` | HTTP client utilities, mock handlers |
| `WeihanLi.Common.Logging` | Logging abstractions and adapters |
| `WeihanLi.Common.Otp` | TOTP/OTP implementation |
| `WeihanLi.Common.Services` | Common service implementations |
| `WeihanLi.Common.Template` | Lightweight template engine |
| `WeihanLi.Extensions` | Shared extension methods (string, collections, etc.) |

## Code Style and Conventions

- **Language**: C# with nullable reference types enabled (`<Nullable>enable</Nullable>`) and implicit usings
- **Naming**: PascalCase for public members; _camelCase for private fields (private static readonly fields may use PascalCase)
- **License header**: Apache License 2.0 header in every source file
- **Formatting**: governed by `.editorconfig` — run `dotnet format` before committing

### Parameter Validation

Always validate parameters using the `Guard` class:

```csharp
public static string Process(string input)
{
    Guard.NotNullOrEmpty(input);
    // implementation
}
```

### Extension Methods

Place extension methods in dedicated files with descriptive names, using the `WeihanLi.Extensions` namespace:

```csharp
namespace WeihanLi.Extensions;

public static class StringExtension
{
    public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);
}
```

### Fluent API Design

Many components expose fluent interfaces:

```csharp
FluentAspects.Configure(options =>
    options.InterceptAll()
           .With<LoggingInterceptor>()
);
```

### Options Pattern

Use the options pattern for configurable components:

```csharp
public sealed class ServiceOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public int Timeout { get; set; } = 30;
}
```

## Testing Guidelines

- **Framework**: xUnit v3 (`xunit.v3`)
- **Location**: `test/WeihanLi.Common.Test/`
- **File naming**: `{ComponentName}Test.cs`
- **Namespace**: `WeihanLi.Common.Test`
- Test method names follow the pattern `MethodName_Scenario_ExpectedResult`

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

Use `[Theory]` + `[InlineData]` for parameterized tests.

## Common Development Tasks

### Adding a New Extension Method

1. Create (or edit) the appropriate file in `src/WeihanLi.Common/Extensions/`
2. Use namespace `WeihanLi.Extensions`
3. Add XML documentation for all public members
4. Write a corresponding test in `test/WeihanLi.Common.Test/`

### Adding a New Helper or Service

1. Define an interface in `Abstractions/` if applicable
2. Implement in `Helpers/` or `Services/`
3. Add configuration options class if configurable
4. Register with DI if applicable
5. Write tests and update samples if the feature is significant

### Working with AOP (Fluent Aspects)

```csharp
// Configure interceptors
FluentAspects.Configure(options =>
{
    options.InterceptMethod<IService>(s => s.Process(Argument.Any<string>()))
           .With<ValidationInterceptor>();
});

// Create proxy
var service = FluentAspects.AspectOptions.ProxyFactory
    .CreateProxy<IService>(new ServiceImplementation());
```

### Database / Data-Access Extensions

```csharp
// Query using ADO.NET extensions
var users = connection.Select<User>(
    "SELECT * FROM Users WHERE Age > @age", new { age = 18 });

// Repository pattern
var repository = new Repository<User>(() => connectionFactory.GetConnection());
var user = repository.Fetch(u => u.Id == userId);
```

## Dependencies

Key NuGet dependencies and their roles:

| Package | Role |
|---|---|
| `Microsoft.Extensions.*` | Configuration, Logging, Hosting, DI abstractions |
| `Newtonsoft.Json` | JSON serialization |
| `Serilog` | Structured logging (via Serilog integration package) |
| `Dapper` | Lightweight ORM / ADO.NET helper |
| `PolySharp` | Polyfill attributes for older .NET targets |
| `xunit.v3` | Test framework |
| `BenchmarkDotNet` | Performance benchmarks |

Package versions are centrally managed in `Directory.Packages.props` — update versions there rather than in individual project files.

## Security

- Validate all inputs using `Guard` utilities
- Dispose resources properly (use `using` statements)
- Use `RandomNumberGenerator` (not `Random`) for cryptographic operations
- Never commit secrets or credentials
- NuGet audit is enabled via MSBuild properties (see `Directory.Build.props` / `Directory.Packages.props` with `<NuGetAudit>true</NuGetAudit>`); do not suppress audit warnings without justification

## Pull Request Guidelines

### Commit Message Conventions

This project follows the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) specification. Every commit message must be structured as:

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

**Common types:**

| Type | When to use |
|---|---|
| `feat` | A new feature (correlates with MINOR in SemVer) |
| `fix` | A bug fix (correlates with PATCH in SemVer) |
| `docs` | Documentation-only changes |
| `style` | Formatting, missing semicolons, etc. — no logic change |
| `refactor` | Code change that is neither a fix nor a feature |
| `perf` | Performance improvement |
| `test` | Adding or correcting tests |
| `build` | Changes to the build system or external dependencies |
| `ci` | Changes to CI/CD configuration files |
| `chore` | Maintenance tasks that don't modify src or test files |

**Breaking changes**: append `!` after the type/scope or add a `BREAKING CHANGE:` footer.

**Examples:**

```
feat(event): add retry support to EventBus
fix(otp): correct TOTP window boundary calculation
docs: update AGENTS.md with PR guidelines
build: bump Newtonsoft.Json to 13.0.4
feat!: remove obsolete IDependencyResolver overloads

BREAKING CHANGE: IDependencyResolver.GetService<T>() overloads that
accepted a string key have been removed. Use keyed services instead.
```

### Required Checks Before Submitting

- `dotnet build` — must compile without errors or warnings
- `dotnet test` — all tests must pass
- `dotnet format --verify-no-changes` — no formatting violations

### PR Title Format

Use the same `<type>[optional scope]: <description>` format as the commit message.

## Debugging and Troubleshooting

- **Build failures**: run `dotnet build --verbosity detailed` for full MSBuild output
- **Test failures on a specific framework**: use `dotnet test -f net8.0` to target one TFM
- **GitHub Actions test logger noise locally**: set `DISABLE_GITHUB_ACTIONS_TEST_LOGGER=true`
- **NuGet restore errors**: verify `Directory.Packages.props` contains the version; do not add `<Version>` in individual `.csproj` files
- **Reflection.Emit issues on netstandard2.0**: the package conditionally references `System.Reflection.Emit` — guard with `#if !NETSTANDARD2_0` or `#if NET8_0_OR_GREATER` as appropriate

## Documentation

- XML documentation is required on all public APIs
- Docs live in `docs/` and are generated with DocFX (`docs/docfx.json`)
- Release notes are in `docs/ReleaseNotes.md`
- Samples in `samples/` demonstrate end-to-end usage patterns
