# Modular Monolith Architecture in C# .NET

## Overview

This documentation describes the architectural principles and implementation details for building a **modular monolith**
application using C# .NET.

The goal is to maintain clear module separation and decouple various concerns while keeping everything within a single deployable application.

The architecture leverages Domain-Driven Design (DDD), Entity Framework Core for data access, MediatR for handling commands, and ASP.NET Core for web services.

## Key Concepts

### Modular Monolith

A **modular monolith** is an architecture where the application is logically divided into distinct modules, each encapsulating a specific business capability.
While all modules reside in the same application process, the separation is maintained at the code and project level.

This approach strikes a balance between fully isolated microservices and a traditional monolithic design
by improving maintainability and scalability within a single deployment unit.

### Modules Structure

Each module in this architecture contains the following layers:

1. **Domain**: Encapsulates the business rules and domain entities, following the principles of Domain-Driven Design (DDD).
2. **Data**: Handles database operations, including the Entity Framework Core context for PostgreSQL.
3. **Application**: Contains the business logic, commands, and their handlers (using MediatR for CQRS patterns).
4. **Contracts**: Defines the DTOs and service contracts for communication between the module's Web APIs and the client applications.
5. **Internal Client**: Facilitates inter-module communication within the monolith without relying on HTTP.
This client provides methods for invoking module logic internally via MediatR commands.
6. **Web Client**: A typed HTTP client for external systems to interact with the module's APIs.
7. **Web**: Exposes the module's APIs through ASP.NET Core controllers and schedules periodic jobs. The controllers typically call MediatR commands from the Application layer.
8. **Messaging**: Contains message contracts for integration with message brokers like RabbitMQ.
9. **External Resources**: Handles interaction with external APIs or services, for example, NOAA for weather data.

### Centralized Composition

To manage the complexity of modules, each module has a `ServiceModule` for registering its services into the Dependency Injection (DI) container.

The composition root for each module is the `Web` project, which combines all other components of the module into an `ApplicationModule`.

Finally, the `Host` project serves as the **composition root** for the entire application, consolidating all `ApplicationModule` instances from the modules.

### Projects in the Solution

1. **Host**: The central ASP.NET Core application that integrates all the modules' APIs and schedules into a single deployable unit.
It registers all services from the `ApplicationModule` of every module.

2. **Front**: A Blazor project used primarily for debugging.
It interacts with the modules through their `WebClient` implementations, enabling calls to the module APIs hosted by the `Host` application.

3. **Framework**: A set of shared libraries that provide utilities and base classes to facilitate module development.

## Architecture Diagram

Below is a visual representation of the modular monolith architecture:

```plaintext
+------------------------------------------------------+
|                  Host (ASP.NET Core)                 |
|  (Composition root for all modules)                  |
|                                                      |
|   +--------------------+   +-------------------+     |
|   | ApplicationModule 1 |  | ApplicationModule 2|    |
|   |  +--------------+   |  |   +--------------+ |    |
|   |  | ServiceModule |  |  |   | ServiceModule ||    |
|   +--| (Module 1)    |--+  +---| (Module 2)    ||    |
|      +--------------+          +--------------+      |
|                                                      |
|   +--------------------+                             |
|   |  WebClient         |                             |
|   |  (External API)    |                             |
|   +--------------------+                             |
+------------------------------------------------------+

Modules
   |
   |-----------------------------------|-----------------------------------|
   |  Module 1                         |   Module 2
   |                                   |
   |  +-------------------------+      |  +-------------------------+
   |  |  Domain                  |     |  |  Domain                  |
   |  +-------------------------+      |  +-------------------------+
   |                                   |
   |  +-------------------------+      |  +-------------------------+
   |  |  Data                    |     |  |  Data                    |
   |  +-------------------------+      |  +-------------------------+
   |                                   |
   |  +-------------------------+      |  +-------------------------+
   |  |  Application             |     |  |  Application             |
   |  +-------------------------+      |  +-------------------------+
   |                                   |
   |  +-------------------------+      |  +-------------------------+
   |  |  Contracts               |     |  |  Contracts               |
   |  +-------------------------+      |  +-------------------------+
   |                                   |
   |  +-------------------------+      |  +-------------------------+
   |  |  Web                     |     |  |  Web                     |
   |  +-------------------------+      |  +-------------------------+
   |                                   |
   |  +-------------------------+      |  +-------------------------+
   |  |  Internal Client         |     |  |  Internal Client         |
   |  +-------------------------+      |  +-------------------------+
   |                                   |
   |  +-------------------------+      |  +-------------------------+
   |  |  Web Client              |     |  |  Web Client              |
   |  +-------------------------+      |  +-------------------------+
   |                                   |
   |  +-------------------------+      |  +-------------------------+
   |  |  Messaging               |     |  |  Messaging               |
   |  +-------------------------+      |  +-------------------------+
   |                                   |
```

## See Also
- [Validation and DDD](https://www.youtube.com/watch?v=mMo8G3gCOtA)
- [Domain-driven design: Most important](https://www.youtube.com/watch?v=JOy_SNK3qj4)
