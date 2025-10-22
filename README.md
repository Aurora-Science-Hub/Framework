<h1 align="center">
    <br>
    Aurora Science Hub Framework
    <br>
</h1>
<div align="center">
    A comprehensive collection of reusable infrastructure packages for building modern .NET 9 applications.
    <br><br>

[![](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![](https://img.shields.io/badge/C%23-13.0-239120)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/Aurora-Science-Hub/Framework/blob/main/LICENSE)

<a href="#getting-started">Getting Started</a> •
<a href="#available-packages">Available Packages</a> •
<a href="#architecture">Architecture</a> •
<a href="#development">Development</a> •
<a href="#code-style">Code Style</a> •
<a href="#testing">Testing</a> •
<a href="#licence">Licence</a> •
<a href="#contributing">Contributing</a>

</div>


## Getting Started

All packages are distributed via NuGet and target **.NET 9.0**. Install individual packages as needed:

```bash
dotnet add package AuroraScienceHub.Framework.Composition
dotnet add package AuroraScienceHub.Framework.AspNetCore
dotnet add package AuroraScienceHub.Framework.EntityFramework
# ... other packages
```

Each package includes its own README with detailed usage instructions and examples.

## Available Packages

### Core Infrastructure

- **[Composition](src/Composition/)** - Modular architecture framework with service and application modules
- **[Configuration](src/Configuration/)** - Type-safe configuration loading with validation
- **[Diagnostics](src/Diagnostics/)** - Health checks, application descriptors, and monitoring utilities
- **[Exceptions](src/Exceptions/)** - Common exception types and error handling patterns
- **[Utilities](src/Utilities/)** - General-purpose utility classes and extension methods

### Web & API

- **[AspNetCore](src/AspNetCore/)** - ASP.NET Core extensions, problem details, security, and routing
- **[Http](src/Http/)** - HTTP client utilities and extensions

### Data Access

- **[Entities](src/Entities/)** - Domain entity interfaces, strong-typed IDs, and DDD patterns
- **[EntityFramework](src/EntityFramework/)** - EF Core extensions with auditing, soft deletes, and migrations
- **[EntityFramework.NpgSql](src/EntityFramework.NpgSql/)** - PostgreSQL-specific DbContext factories and conventions
- **[ClickHouse](src/ClickHouse/)** - ClickHouse database integration

### Caching & Performance

- **[Caching](src/Caching/)** - Hybrid cache extensions and utilities

### Logging & Observability

- **[Logging.OpenTelemetry](src/Logging.OpenTelemetry/)** - OpenTelemetry integration for distributed tracing

### AI & Machine Learning

- **[Ai](src/Ai/)** - OpenAI GPT and DeepL translation service integrations
- **[Ocr](src/Ocr/)** - Optical Character Recognition utilities

### Serialization

- **[Json](src/Json/)** - JSON serialization extensions and converters

## Architecture

The framework follows these principles:

- **Modular Design** - Each package is self-contained and can be used independently
- **Clean Architecture** - Clear separation of concerns with minimal coupling
- **Domain-Driven Design** - Support for DDD patterns and practices
- **Type Safety** - Strong typing with nullable reference types enabled
- **Modern .NET** - Leverages latest .NET 9 features and patterns

## Development

### Prerequisites

- .NET 9.0 SDK or later
- IDE with C# support (Rider, Visual Studio, VS Code)

### Building the Solution

```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Run tests
dotnet test

# Create NuGet packages
dotnet pack
```

## Code Style

The solution uses [EditorConfig](.editorconfig) based on [Azure SDK .NET](https://github.com/Azure/azure-sdk-for-net/blob/main/.editorconfig) to maintain consistent code style across all packages.

### Formatting Commands

```bash
# Format code before committing
dotnet format

# Verify code style compliance
dotnet format --verify-no-changes
```

### Code Quality Standards

- Nullable reference types enabled
- Warnings treated as errors
- Latest C# language version
- Code style enforcement in build
- Embedded debug symbols in packages

## Testing

Unit tests are located in the `tests/UnitTests/` directory. The framework uses:

- xUnit for test execution
- AutoFixture with AutoMoq for test data generation
- Moq for mocking

## License

See [LICENSE](LICENSE) and [PackageLicenses.txt](PackageLicenses.txt) for details.

## Contributing

When contributing to this repository:

1. Follow the established code style (enforced by EditorConfig)
2. Run `dotnet format` before committing
3. Ensure all tests pass
4. Update relevant README files for your changes
5. Keep packages focused and loosely coupled

---

**Note**: This framework is designed for Aurora Science Hub applications but can be used in any modern .NET project requiring robust infrastructure components.
