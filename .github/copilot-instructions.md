# WeihanLi.Common repository instructions

## Build, test, and format

Use the .NET SDKs expected by CI: **10.0.x** and **11.0.x preview**. There is no `global.json`.

```bash
dotnet build
dotnet test
dotnet build.cs
dotnet format --verify-no-changes
dotnet format
```

Run the test project directly when you want a narrower loop:

```bash
dotnet test test\WeihanLi.Common.Test\WeihanLi.Common.Test.csproj
```

Run a single test with a filter, for example:

```bash
dotnet test test\WeihanLi.Common.Test\WeihanLi.Common.Test.csproj --filter "FullyQualifiedName~WeihanLi.Common.Test.EventsTest.EventBusTest.MainTest"
```

If the GitHub Actions test logger gets in the way locally, set `DISABLE_GITHUB_ACTIONS_TEST_LOGGER=true`.

## High-level architecture

- This repo publishes four packages with a clear dependency ladder:
  - `WeihanLi.Core`: dependency-light, AOT-friendly primitives, helpers, models, compressors, OTP, and extension methods.
  - `WeihanLi.Common`: the heavier integration layer on top of `WeihanLi.Core` for DI, AOP, logging, data access, HTTP, JSON, configuration, templating, and eventing.
  - `WeihanLi.Common.Logging.Serilog`: adapter package that plugs Serilog into `WeihanLi.Common.Logging`.
  - `WeihanLi.Extensions.Hosting`: hosting-specific helpers layered on top of `WeihanLi.Common`.
- `src\WeihanLi.Core\WeihanLi.Core.csproj` deliberately uses `RootNamespace` **`WeihanLi.Common`**. Core APIs still live in the `WeihanLi.Common.*` namespace even though the implementation now sits in the `WeihanLi.Core` package.
- `src\WeihanLi.Common\TypeForwarding.cs` preserves compatibility for APIs that moved into `WeihanLi.Core`. When relocating shared primitives between packages, preserve type forwarding and the existing namespace story.
- The main extension points are fluent or DI-driven:
  - `WeihanLi.Common.Aspect` configures interception through `FluentAspects`.
  - `WeihanLi.Common.Event` provides in-process eventing plus queue/store abstractions.
  - `WeihanLi.Common.Data` provides repository and ADO.NET helper layers.
  - `WeihanLi.Common.DependencyInjection` and `DependencyResolver` bridge the library's own container abstractions with `IServiceCollection` / `IServiceProvider`.
- The test project references `WeihanLi.Common`, so many tests exercise both the umbrella package and type-forwarded core APIs through the shared namespace surface.
- `build.cs` is the CI-aligned entry point for build/test/pack orchestration. GitHub Actions runs `dotnet build.cs` on Windows, macOS, and Linux.

## Key conventions

- Keep low-dependency, AOT-friendly functionality in `src\WeihanLi.Core`; use `src\WeihanLi.Common` only for heavier integrations.
- Public APIs in source packages should keep XML documentation enabled and source files should retain the existing Apache license header style.
- Use `Guard` helpers for parameter validation instead of ad hoc checks.
- Put extension methods in dedicated files under an `Extensions\` folder and use the `WeihanLi.Extensions` namespace.
- This repo multi-targets `netstandard2.0`, `net8.0`, `net10.0`, and `net11.0` depending on the package. Prefer framework guards for TFM-specific behavior instead of dropping compatibility.
- Package versions are centrally managed in `Directory.Packages.props`; do not add per-project package versions unless the repo already does so.
- Some APIs are generated from T4 templates. If you need to change `ServiceContainerBuilderExtensions.generated.cs`, `DbCommandExtension.generated.cs`, or `DbConnectionExtension.generated.cs`, edit the corresponding `.tt` file instead of only patching generated output.
- Tests follow the existing `*Test.cs` naming and usually live under `WeihanLi.Common.Test` plus a feature sub-namespace such as `EventsTest` or `ExtensionsTest`.
- `samples\DotNetCoreSample` and `samples\AspNetCoreSample` are the best references for end-to-end usage patterns when changing DI, logging, or eventing behavior.

<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan
<!-- SPECKIT END -->
