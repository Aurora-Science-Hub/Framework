using AuroraScienceHub.Framework.Utilities.System;
using Scriban.Runtime;

namespace AuroraScienceHub.Framework.ModuleCreator;

internal sealed class ModuleModel : ScriptObject
{
    public ModuleModel(
        string companyName,
        string serviceName,
        string moduleName,
        bool addDbContext)
    {
        Add("CompanyName", companyName);
        Add("ServiceName", serviceName);
        Add("ModuleName", moduleName);
        Add("AddDbContext", addDbContext);
    }

    public static string KebabCaseLower(string text)
    {
        return text.PascalToKebabCase()
            .Trim()
            .ToLower();
    }
}
