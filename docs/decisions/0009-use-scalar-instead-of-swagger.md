# Use Scalar Instead of Swagger

## Status

* Status: **accepted**
* Deciders: [alex1ozr](https://github.com/alex1ozr)
* Date: 01.01.2025
* Technical Story: SPACE-206

## Context and Problem Statement

With the release of .NET 9, Microsoft has removed the Swagger UI (Swashbuckle) from the default Web API templates due to
maintenance issues and a desire to simplify project templates. ([GitHub Issue](https://github.com/dotnet/aspnetcore/issues/54599))
This change necessitates exploring alternative solutions for API documentation and testing.
One such alternative is Scalar, an open-source interactive API documentation tool. ([Scalar](https://scalar.com/))

## Considered Options

- Use Scalar
- Continue using Swagger

## Decision Outcome

**Chosen option: "Use Scalar".** This decision is based on several factors:

- **Modern User Interface**: Scalar provides a clean and responsive UI for API documentation, enhancing the developer experience. ([Scalar](https://scalar.com/))
- **Seamless Integration**: Scalar integrates smoothly with existing tools and workflows, reducing setup and maintenance complexities. ([Scalar](https://scalar.com/))
- **Enhanced Features**: Scalar offers features such as an integrated API playground, environment variables, dynamic parameters, and support for various API frameworks, making it a versatile tool for API documentation and testing. ([GitHub](https://github.com/scalar/scalar/blob/main/README.md))
- **Open-Source and Actively Maintained**: Being open-source, Scalar benefits from community contributions and active maintenance, ensuring continuous improvements and updates. ([GitHub](https://github.com/scalar/scalar))

## Links

- [Scalar Documentation](https://github.com/scalar/scalar)
