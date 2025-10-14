# Use Minimal API instead of classic Controllers

## Context and Problem Statement

The common practice is to use classic Controllers for Web API design.
At the same time, Microsoft provides another solution for this purpose — Minimal API.
Minimal API is Microsoft’s attempt to provide a simplified, more streamlined approach for developing JSON-based APIs using ASP.NET Core.
They aim to cut down on boilerplate code while still offering an impressive set of features.

## Considered Options

* Use Minimal API
* Use classic Controllers
* Use FastEndpoints

## Decision Outcome

Chosen option: "Use Minimal API". There are several reasons:
* Simplified Syntax and Less Boilerplate: Minimal APIs allow you to define endpoints in a concise, direct manner with fewer lines of code compared to traditional controllers, reducing the need for classes, attributes, and action methods.
* Improved Performance: With fewer abstractions and reduced overhead (no need for controller classes, routing, or unnecessary middleware), Minimal APIs can offer better performance, especially for microservices or lightweight web applications.
* Faster Development: Minimal APIs are perfect for quickly building small services or prototypes, as they allow you to focus directly on defining routes and logic without the extra layers of MVC and DI (Dependency Injection) setup, speeding up development.
* Reduced Complexity: Minimal APIs work well in scenarios where you don't need the full MVC structure (like complex routing, model binding, or views), making your codebase cleaner, more maintainable, and easier to understand.

## Links

* [Benchmarks](https://steven-giesel.com/blogPost/698c45c3-58c5-4157-b4da-2cde4e27862e)
* [Minimal API usage example](https://dev.to/milanjovanovictech/automatically-register-minimal-apis-in-aspnet-core-m37)
* [More about Minimal API pt.1](https://www.youtube.com/watch?v=GCuVC_qDOV4)
* [More about Minimal API pt.2](https://jonathancrozier.com/blog/exploring-asp-dot-net-core-minimal-apis-should-i-use-them-in-production)

