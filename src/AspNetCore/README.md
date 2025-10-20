# AuroraScienceHub.Framework.AspNetCore

ASP.NET Core extensions including web application module composition, problem details handling, and security utilities.

## Overview

Extends the Composition framework with ASP.NET Core-specific functionality, providing web application modules and standardized error handling.

## Key Features

- 🌐 **Web Application Modules** - Extend composition framework for web apps
- 📋 **Problem Details** - RFC 7807 compliant error responses
- 🛡️ **Security Utilities** - Authentication and authorization helpers
- 🔀 **Routing Extensions** - Simplified endpoint configuration

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.AspNetCore
```

## Usage

### Web Application Module

```csharp
public class UserWebModule : WebApplicationModuleBase<UserModuleOptions>
{
    public UserWebModule()
        : base("User", new ServiceModuleBase[]
        {
            new UserServiceModule()
        })
    {
    }

    public override void ConfigureApplication(
        IApplicationBuilder app,
        IConfiguration configuration)
    {
        // Configure middleware pipeline
        app.UseAuthentication();
        app.UseAuthorization();
    }
}

// Register in Program.cs
var builder = WebApplication.CreateBuilder(args);
var userModule = new UserWebModule();
userModule.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();
userModule.ConfigureApplication(app, app.Configuration);
```

### Problem Details Error Handling

```csharp
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        var problemDetails = exception switch
        {
            ValidationException ex => new ProblemDetails
            {
                Status = 400,
                Title = "Validation Error",
                Detail = ex.Message
            },
            EntityNotFoundException ex => new ProblemDetails
            {
                Status = 404,
                Title = "Not Found",
                Detail = ex.Message
            },
            _ => new ProblemDetails
            {
                Status = 500,
                Title = "An error occurred"
            }
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});
```

### Module Options

```csharp
public class UserModuleOptions : ApplicationModuleOptionsBase
{
    public bool EnableEmailVerification { get; set; } = true;
}

// appsettings.json
{
  "Modules": {
    "User": {
      "EnableEmailVerification": true,
      "EnableConsumers": true
    }
  }
}
```

## Best Practices

- Organize by feature with one module per business domain
- Use Problem Details for standardized error responses
- Configure middleware in correct order (auth before authz)
- Handle exceptions globally with middleware
- Version your APIs with separate modules

## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.Composition` - Core module composition
- `AuroraScienceHub.Framework.Diagnostics` - Health checks
