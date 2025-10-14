# Guidelines

## General
### Useful resources
- [Radio DotNet podcast, ep.83](https://radiodotnet.mave.digital/ep-83)
- [Radio DotNet podcast, ep.88](https://radiodotnet.mave.digital/ep-88)

## C#

- Async/Await
  - Use `ConfigureAwait(false)` in library code and `ConfigureAwait(true)` in the UI code
  - Use `Async` suffix for async methods
  - Always pass `CancellationToken` to async methods
- Logging
  - Use `ILogger<YourClass>` interface for injecting loggers
  - Use [structured logging](https://github.com/serilog/serilog/wiki/Structured-Data) (e.g. `LogInformation("Processed {Count} items", count)`)
    - Don't use string interpolation or concatenation

### Documentation & materials
- [MSDN - Common C# code conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions?redirectedfrom=MSDN)
- [MSDN - C# identifier names](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names)
- [.NET Azure SDK Design Guidelines](https://azure.github.io/azure-sdk/dotnet_introduction.html)
- [MSDN - Naming guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines)
- [Habr.com - Делай нейминг как сеньор](https://habr.com/ru/companies/dododev/articles/714512/)
- [YouTube - Correcting Common Async/Await Mistakes in .NET 8](https://www.youtube.com/watch?v=GQYd6MWKiLI&t=2085s)

## REST API
It is important to follow the best practices for designing RESTful web APIs.

The following resources can help you with that:
- [MSDN - RESTful web API design](https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design)
- [Microsoft Azure REST API Guidelines](https://github.com/microsoft/api-guidelines/blob/vNext/azure/Guidelines.md)
- [Zalando RESTful API and Event Guidelines](https://opensource.zalando.com/restful-api-guidelines/)
- [Jeffrey Richter - Welcome to the 'Designing & Versioning HTTP/REST APIs' video series](https://www.youtube.com/watch?v=9Ng00IlBCtw&list=PL9XzOCngAkqs4m0XdULJu_78nM3Ok3Q65)
- [Martin Fowler - Steps toward the glory of REST](https://martinfowler.com/articles/richardsonMaturityModel.html)
- [Irina Scurtu - Versioning REST APIs](https://irina.codes/versioning-rest-apis/)
- [5 Tips for API Design.](https://codeopinion.com/want-to-build-a-good-api-here-are-5-tips-for-api-design/)
- [How to (and how not to) design REST APIs](https://github.com/stickfigure/blog/wiki/How-to-(and-how-not-to)-design-REST-APIs?ref=vladimir-ivanov-dev-blog)
- [Dylan Beattie — Real World REST and Hands-On Hypermedia](https://www.youtube.com/watch?v=kPrTMj-BK14)

### We agreed to use the following guidelines for REST API design
- Use plural nouns for resources
- Use [Problem Details](https://datatracker.ietf.org/doc/html/rfc9457) for error responses
- Use kebab-case for URLs
- Use camelCase for JSON properties
- Don't generate IDs on the client side
- [Use strings as IDs](../../docs/decisions/0001-use-strings-as-ids.md)
  - [Use prefixes in IDs](../../docs/decisions/0002-use-prefixes-in-ids.md)
- Don't use PATCH verb
- Don't specify known status codes (400, 500) in controller
- Serialize enums as strings
- Serialize dates using [ISO 8601](https://learn.microsoft.com/en-us/rest/api/storageservices/formatting-datetime-values) format
- Use idempotency keys for operations that are not idempotent
- Don't use versioning in routes
