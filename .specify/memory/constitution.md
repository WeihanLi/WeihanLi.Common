<!--
## Sync Impact Report

**Version change**: (none) → 1.0.0
**Added sections**: Core Principles (I–VI), Technology Constraints, Development Workflow, Governance
**Removed sections**: N/A (initial creation)
**Templates requiring updates**:
  - `.specify/templates/plan-template.md` ✅ — Constitution Check section aligns with principles below
  - `.specify/templates/spec-template.md` ✅ — No mandatory sections changed; constraints reflected
  - `.specify/templates/tasks-template.md` ✅ — Task categories (tests, security, docs) align with principles
  - `.specify/templates/commands/*.md` ✅ — No outdated agent-specific references identified
**Deferred TODOs**: None
-->

# WeihanLi.Common Constitution

## Core Principles

### I. AOT-Friendly Core (NON-NEGOTIABLE)

`WeihanLi.Core` MUST remain AOT-compatible and dependency-light. It MUST NOT introduce
package dependencies beyond the BCL. Any API added to `WeihanLi.Core` MUST compile and
run correctly under NativeAOT and trimming. Heavy integrations (DI, logging, HTTP, AOP)
belong in `WeihanLi.Common` or dedicated integration packages.

### II. Multi-Target Compatibility

Every source package MUST build and pass tests on ALL declared target frameworks
(`netstandard2.0`, `net8.0`, `net10.0`, `net11.0` for `WeihanLi.Core` and
`WeihanLi.Common`). Framework-specific code MUST be guarded with `#if` directives
(e.g., `#if NET8_0_OR_GREATER`). APIs missing from `netstandard2.0` MUST use compatible
alternatives; `Reflection.Emit` and similar unavailable APIs MUST be conditionally excluded.

### III. Test-Driven Quality (NON-NEGOTIABLE)

All public API changes, new features, and bug fixes MUST be accompanied by xUnit v3 tests
in `test/WeihanLi.Common.Test/`. Tests MUST follow the `MethodName_Scenario_ExpectedResult`
naming convention. Parameterized cases MUST use `[Theory]` + `[InlineData]`. The test suite
MUST pass (`dotnet test`) before any change is merged. No regression is acceptable.

### IV. Minimal Dependency Footprint

Packages MUST NOT pull in transitive dependencies without deliberate justification. All
package versions are centrally managed in `Directory.Packages.props`; individual `.csproj`
files MUST NOT declare `<Version>` metadata. New package references require explicit
approval and MUST be audited for security vulnerabilities (NU1901–NU1904 are errors).
Complexity MUST be justified — prefer extending existing helpers over adding new abstractions.

### V. Security & Input Validation

All public APIs accepting parameters MUST use `Guard` utilities for input validation.
Cryptographic and security-sensitive randomness MUST use `RandomNumberGenerator`, never
`Random`. Resources MUST be disposed properly using `using` declarations or equivalent
ownership patterns. Secrets, credentials, tokens, and local machine configuration MUST
never be committed to source control.

### VI. Consistent API Design & Documentation

Public APIs MUST use PascalCase naming. Private fields MUST use `_camelCase`. All public
APIs in source packages MUST carry XML documentation comments. Fluent configuration APIs
MUST follow the patterns established in `FluentAspects` and related components. Extension
methods MUST reside in dedicated files under the appropriate `Extensions/` folder and use
the `WeihanLi.Extensions` namespace. T4-generated files MUST be regenerated from their
`.tt` source when the generated API changes.

## Technology Constraints

- **Language**: C# with `LangVersion = preview`; preview-only syntax MUST NOT be used
  when older target frameworks require compatibility.
- **Nullable reference types**: Enabled globally; all new code MUST be null-safe.
- **Implicit usings**: Enabled; redundant explicit usings MUST be removed.
- **Source files**: MUST include the Apache License 2.0 header used throughout the repo.
- **Build tooling**: .NET SDK 10.0.x (stable) + 11.0.x (preview) required for full builds.
  CI runs both Azure Pipelines and GitHub Actions; local commands MUST match CI behavior.
- **Formatting**: `dotnet format --verify-no-changes` MUST pass before merge.

## Development Workflow

- Changes MUST be submitted as pull requests targeting `master` (release) or `dev` (development).
- Commit messages and PR titles MUST follow Conventional Commits
  (`feat`, `fix`, `docs`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`).
  Breaking changes MUST append `!` or include a `BREAKING CHANGE:` footer.
- The required pre-merge checks are: `dotnet build`, `dotnet test`,
  `dotnet format --verify-no-changes`.
- Documentation MUST be updated for any public API change: XML docs in source,
  package README files under `src/*/README.md` for package-level behavior changes,
  and `docs/ReleaseNotes.md` for user-facing changes.
- Samples in `samples/` MUST be kept consistent with the public API they demonstrate.

## Governance

This constitution supersedes all other practices and guidelines in the repository. Any
amendment MUST be documented, versioned (following semantic versioning rules below), and
merged via the standard PR process. All PRs MUST verify compliance with the Core Principles
before approval. Introduced complexity MUST be justified against existing patterns; refer to
`AGENTS.md` for runtime agent-specific development guidance.

**Amendment procedure**: Open a PR that edits this file, increments the version, and
updates the `Last Amended` date. At least one maintainer approval is required. Backward-
incompatible removals or redefinitions of principles increment MAJOR; new sections or
materially expanded guidance increment MINOR; clarifications and wording fixes increment PATCH.

**Version**: 1.0.0 | **Ratified**: 2026-06-02 | **Last Amended**: 2026-06-02
