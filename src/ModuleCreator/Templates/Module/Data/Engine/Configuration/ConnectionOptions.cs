using {{CompanyName}}.Framework.EntityFramework;

namespace {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Data.Engine.Configuration;

/// <summary>
/// Db connection options
/// </summary>
public sealed class ConnectionOptions : IConnectionOptions
{
    public string? SpaceWeather { get; set; }

    public string GetConnectionString() =>
        SpaceWeather
        ?? throw new ArgumentException($"Connection string «{nameof(SpaceWeather)}» not found");
}
