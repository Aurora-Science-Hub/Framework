# Tests

## Tests in the solution

### [Architectural tests](../../tests/ArchitectureTests)
Architectural tests for the whole solution.
  - [Framework projects](../../src/Framework) references tests.
  - [Module projects](../../src/Modules) references tests.

### Unit tests
  - [Unit tests for the Host project](../../tests/UnitTests) (including all modules).
    - Dependency injection tests for all web API controllers, background jobs, and MediatR handlers.
    - Tests for each module's services, handlers and other classes.
  - [Unit tests for the Framework](../../tests/Framework/UnitTests) classes, helpers, and extensions.

### [Integration tests](../../tests/IntegrationTests)
Integration tests for the Host project (including all modules).
  - Each module has its own integration tests.
  - All external dependencies run in Docker containers using [TestContainers](https://www.testcontainers.org).

## Common rules

### Basics
- Use the [Arrange-Act-Assert](https://learn.microsoft.com/en-us/visualstudio/test/unit-test-basics?view=vs-2019#write-your-tests) pattern.
- Handlers, Services, Validators must have unit tests.
  - Use mocks to isolate the unit under test from its dependencies.
  - Don't test private methods.
- API (Module) Clients, Background jobs must have integration tests.

### Naming convention
Use the following naming convention for your tests:
- [MSDN - Naming your tests](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices#naming-your-tests)

#### Example
- \<MethodName\>\_\<Precondition\>\_\<ExpectedBehavior\>
  - GetRawDataRecords_WithValidProcessId_ReturnsRecords
  - GetRawDataRecords_WithInvalidProcessId_Throws

## See also
- [Unit test basics](https://docs.microsoft.com/en-us/visualstudio/test/unit-test-basics)
- [Unit Testing Principles](https://www.youtube.com/watch?v=LkrqqpkKIXE)
