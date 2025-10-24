# AuroraScienceHub.Framework.Exceptions

Comprehensive exception library for .NET applications with standardized exception types for common scenarios.

## Overview

Provides well-defined exception classes that follow best practices. All exceptions inherit from `FrameworkException` for consistent error handling.

## Key Features

- **Standardized Exceptions** - Common exception types for typical scenarios
- **Hierarchical Structure** - All exceptions inherit from `FrameworkException`
- **Validation Helpers** - Built-in guard clauses and validation utilities
- **Rich Context** - Structured exception reasons with detailed information

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.Exceptions
```

## Exception Types

### ValidationException

```csharp
// Basic validation
throw new ValidationException("Invalid input data");

// With guard clauses
ValidationException.ThrowIf(age < 0, "Age must be positive");
ValidationException.ThrowIfNot(email.Contains("@"), "Invalid email");
var user = ValidationException.ThrowIfNull(userInput, "User cannot be null");

// With validation reasons
var reasons = new ValidationExceptionReason();
reasons.Add("Email", "Invalid email format");
throw new ValidationException("Validation failed", reasons);
```

### EntityNotFoundException

```csharp
throw new EntityNotFoundException($"User {userId} not found");
```

### AccessDeniedException

```csharp
throw new AccessDeniedException("Insufficient permissions to delete resource");
```

### UnexpectedException

```csharp
throw new UnexpectedException("Unexpected state in payment processing");
```

## Usage Example

```csharp
public class UserService
{
    public User GetUser(int id)
    {
        var user = _repository.FindById(id);
        if (user == null)
            throw new EntityNotFoundException($"User {id} not found");
        return user;
    }

    public void CreateUser(string email, int age)
    {
        ValidationException.ThrowIfNull(email, "Email is required");
        ValidationException.ThrowIf(age < 18, "User must be 18 or older");
        // Create user...
    }
}
```

## Exception Handling

```csharp
try
{
    var result = _service.ProcessRequest(request);
    return Ok(result);
}
catch (ValidationException ex)
{
    return BadRequest(new { error = ex.Message, reasons = ex.Reason });
}
catch (EntityNotFoundException ex)
{
    return NotFound(new { error = ex.Message });
}
catch (AccessDeniedException ex)
{
    return Forbid(ex.Message);
}
```


## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.AspNetCore` - Automatic exception to HTTP response conversion
- `AuroraScienceHub.Framework.Entities` - Domain entity interfaces
