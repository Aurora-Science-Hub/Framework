using OpenTelemetry.Resources;

namespace AuroraScienceHub.Framework.Logging.OpenTelemetry;

public static class ResourceBuilderExtensions
{
    /// <summary>
    /// Name of the property to save the name of the service
    /// </summary>
    private const string ApplicationPropertyName = "ApplicationName";

    /// <summary>
    /// Name of the property to save the namespace in which the service is running
    /// </summary>
    private const string NamespacePropertyName = "HostNamespace";

    public static ResourceBuilder AddApplicationName(this ResourceBuilder resourceBuilder, string applicationName)
    {
        return resourceBuilder.AddAttributes(new Dictionary<string, object>
        {
            { ApplicationPropertyName, applicationName }
        });
    }

    public static ResourceBuilder AddNamespace(this ResourceBuilder resourceBuilder, string hostNamespace)
    {
        return resourceBuilder.AddAttributes(new Dictionary<string, object>
        {
            { NamespacePropertyName, hostNamespace }
        });
    }
}
