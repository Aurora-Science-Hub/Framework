Based on https://github.com/microsoft/skills/blob/main/Agents.md (MIT License)

# Agent Guidelines

This file provides instructions for AI coding agents working with this repository.

## Repository Overview

**AuroraScienceHub.Framework** — a collection of reusable NuGet infrastructure packages for building modern .NET applications.
All packages are published under the `AuroraScienceHub.Framework.*` namespace and distributed via NuGet.org.

This is a **library repository** — it produces NuGet packages consumed by downstream applications, not a runnable application itself.

---

## Core Principles

These principles reduce common LLM coding mistakes. Apply them to every task.

### 1. Think Before Coding

**Don't assume. Don't hide confusion. Surface tradeoffs.**

- State assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them — don't pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- If something is unclear, stop. Name what's confusing. Ask.

### 2. Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that wasn't requested.
- No error handling for impossible scenarios.
- If you write 200 lines, and it could be 50, rewrite it.

**The test:** Would a senior engineer say this is overcomplicated? If yes, simplify.

### 3. Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:
- Don't "improve" adjacent code, comments, or formatting.
- Don't refactor things that aren't broken.
- Match existing style, even if you'd do it differently.
- If you notice unrelated dead code, mention it — don't delete it.

When your changes create orphans:
- Remove imports/variables/functions that YOUR changes made unused.
- Don't remove pre-existing dead code unless asked.

**The test:** Every changed line should trace directly to the user's request.

### 4. Goal-Driven Execution (TDD)

**Define success criteria. Loop until verified.**

Transform tasks into verifiable goals:

| Instead of... | Transform to... |
|---------------|-----------------|
| "Add validation" | "Write tests for invalid inputs, then make them pass" |
| "Fix the bug" | "Write a test that reproduces it, then make it pass" |
| "Refactor X" | "Ensure tests pass before and after" |

For multi-step tasks, state a brief plan:
```
1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.

---

## Project Structure

```
src/
├── <PackageName>/               # Each subfolder is a separate NuGet package
│   └── <PackageName>.csproj     # Inherits shared props from Directory.Build.props
tests/
├── UnitTests/                   # xUnit v3 + AutoFixture + Shouldly

Directory.Build.props            # Root build properties (TFM, lang, analyzers)
Directory.Build.targets          # Build targets
Directory.Packages.props         # Central NuGet version management
Framework.slnx                   # Solution file
global.json                      # SDK version pinning
```

Each package under `src/` maps to a NuGet package named `AuroraScienceHub.Framework.<PackageName>`.
Refer to the individual `README.md` within each package directory for details.

---

## Development Workflow

### Prerequisites

- .NET 10 SDK (see `global.json` for exact version)

### Commands

```bash
dotnet restore          # Restore packages
dotnet build            # Build all projects
dotnet test             # Run unit tests
dotnet pack             # Create NuGet packages
dotnet format           # Format code per .editorconfig before every commit
```

### Adding a New Package

1. Create a new directory under `src/` with the package name
2. Create a `.csproj` file — it will inherit shared properties from `Directory.Build.props`
3. Add a reference in `Framework.slnx`

---

## Conventions

### Clean Code Checklist

Before completing any code change:

- [ ] Functions do one thing
- [ ] No unnecessary dependencies added
- [ ] New functionality has corresponding unit tests
- [ ] Names are descriptive and intention-revealing
- [ ] No magic numbers or strings (use constants)
- [ ] Error handling is explicit (no empty catch blocks)
- [ ] No commented-out code
- [ ] Nullable annotations are correct
- [ ] New public APIs have XML documentation
- [ ] Builds without warnings (`dotnet build`)
- [ ] Tests pass (`dotnet test`)
- [ ] Code formatted per `.editorconfig` (`dotnet format --verify-no-changes`)

### Project setting (inherited from `Directory.Build.props`)

- **Target framework:** `net10.0`
- **Language version:** `latest`
- **Nullable reference types:** enabled globally
- **Warnings as errors:** enabled (`TreatWarningsAsErrors`)
- **Code style:** defined in `.editorconfig` (based on Azure SDK .NET guidelines)
- **PDB:** embedded in assemblies

### Code Style

- Prefer `async/await` for all I/O operations

### Naming

- Namespaces: `AuroraScienceHub.Framework.<PackageName>`
- Follow standard .NET naming conventions (PascalCase for public members, camelCase for locals)
- Interface prefix: `I` (e.g., `IEntity`, `IModule`)

### API Design

- Follow [Microsoft .NET Library Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- Prefer extension methods for optional functionality
- Use `IServiceCollection` extensions for DI registration
- Mark public API surface intentionally — don't expose internals accidentally
- Use XML documentation comments on all public types and members

### Dependencies

- All NuGet package versions are managed centrally in `Directory.Packages.props`
- Never specify versions in individual `.csproj` files — use `<PackageReference Include="..." />` without `Version`
- Minimize external dependencies; prefer BCL types where possible
- When adding a new dependency, justify it

---

## Package Versioning

- Versioning is managed by **MinVer** based on git tags
- Base version is defined in `Directory.Build.props` as `PackageBaseVersion`
- Release versions are produced from semver git tags (e.g., `10.0.3`)
- Pre-release versions are generated automatically on non-main branches

**Do not** manually set `<Version>` or `<PackageVersion>` in project files.

---

## Testing

- **Framework:** xUnit v3
- **Assertions:** Shouldly
- **Mocking:** Moq + AutoFixture.AutoMoq
- **Test data:** AutoFixture

### Conventions

- Test project: `tests/UnitTests/`
- Test class naming: `{ClassUnderTest}Tests`
- Test method naming: `{Method}_{Scenario}_{ExpectedBehavior}` or descriptive names
- Follow Arrange-Act-Assert pattern

```csharp
[Fact]
public void Parse_ValidInput_ReturnsExpectedResult()
{
    // Arrange
    var input = "test-value";

    // Act
    var result = MyParser.Parse(input);

    // Assert
    result.ShouldNotBeNull();
    result.Value.ShouldBe("test-value");
}
```

### Running Tests

```bash
dotnet test                              # All tests
dotnet test --filter "ClassName=FooTests" # Specific class
```

---

## CI/CD

GitHub Actions workflow (`.github/workflows/dotnet.yml`):

| Trigger | Behavior |
|---------|----------|
| Push to `main` | Build → Test → Pack → Push to **NuGet.org** |
| Push to other branches | Build → Test → Pack → Push pre-release to **GitHub Packages** |
| Semver tag (`*.*.*`) | Produces stable version packages |

---

## Do's and Don'ts

### Do

- ✅ Use central package management (`Directory.Packages.props`) for all dependencies
- ✅ Follow existing patterns in the codebase
- ✅ Use XML doc comments on public API surface
- ✅ Use `internal` and `sealed` modifiers by default; only expose what consumers need
- ✅ Validate changes compile with warnings-as-errors enabled
- ✅ Use async/await for all I/O operations
- ✅ Write tests for new functionality before or alongside implementation
- ✅ Keep functions small and focused
- ✅ Keep packages focused — one responsibility per package

### Don't

- ❌ Suppress warnings with `#pragma` or `<NoWarn>` without justification
- ❌ Add `Version` attributes to individual `.csproj` files
- ❌ Modify `Directory.Build.props` or `Directory.Packages.props` without explicit request
- ❌ Add dependencies that duplicate BCL functionality
- ❌ Add dependencies without justification
- ❌ Leave empty catch blocks or swallow exceptions silently
- ❌ Use `dynamic` or reflection where generics/interfaces suffice
- ❌ Break public API contracts (this is a library — consumers depend on stability)
- ❌ Skip nullable annotations on public APIs

---

## Success Indicators

These principles are working if you see:

- Fewer unnecessary changes in diffs
- Fewer rewrites due to overcomplication
- Clarifying questions come before implementation (not after mistakes)
- Clean, minimal PRs without drive-by refactoring
- Tests that document expected behavior
