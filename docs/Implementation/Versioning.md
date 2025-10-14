# Versioning

Application versioning is based on the [Semantic Versioning](https://semver.org) specification and Microsoft's [Versioning Guidelines](https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#12-versioning).

## Implementation specifics
- Application uses [Asp.Versioning.*](https://github.com/dotnet/aspnet-api-versioning) libraries for API versioning.
- The versioning is done via the `api-version` query parameter.
  - It makes resource paths constant and allows clients to choose the API version.
- The default version is `1.0`.

## See also
- [Radio DotNet podcast, ep.88](https://radiodotnet.mave.digital/ep-88)
