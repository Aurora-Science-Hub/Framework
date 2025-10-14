using {{CompanyName}}.Framework.Composition;
using {{CompanyName}}.Framework.Utilities.Configuration;

namespace {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Web.Hosting;

public sealed class ModuleOptions: ApplicationModuleOptionsBase, IOptionDescription
{
    public static string OptionKey => $"{OptionSectionBase}:{{ModuleName}}";
}
