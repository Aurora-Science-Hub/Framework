using System.Diagnostics;
using System.Text;
using Scriban;

namespace AuroraScienceHub.Framework.ModuleCreator;

internal static class ModuleHelper
{
    public static void CreateNew(string moduleName, bool addDbContext)
    {
        Log($"Creating new module {moduleName}...");

        const string companyName = "AuroraScienceHub";
        var rootDir = AppContext.BaseDirectory;

        var templateDir = Path.Combine(rootDir, "Templates", "Module");
        var solutionFile = FindUpSolutionFilePath(rootDir);
        var serviceName = Path.GetFileNameWithoutExtension(solutionFile);
        var solutionDir = Path.GetDirectoryName(solutionFile) ?? throw Error("Solution dir not found");
        var moduleDir = Path.Combine(solutionDir, "src", "Modules", moduleName);

        CopyFilesRecursively(templateDir, moduleDir, addDbContext);

        var model = new ModuleModel(companyName, serviceName, moduleName, addDbContext);
        RenderDirectoryTemplate(moduleDir, model);

        AddToSolution(solutionFile, moduleName, moduleDir);
    }

    private static void Log(string message) => Console.WriteLine(message);
    private static Exception Error(string message) => new InvalidOperationException(message);

    private static void AddToSolution(string solutionFile, string moduleName, string moduleDir)
    {
        var projectFiles = Directory.GetFiles(moduleDir, "*.csproj", SearchOption.AllDirectories);

        var command = $"sln {solutionFile} add --solution-folder \"src/Modules/{moduleName}\" {string.Join(" ", projectFiles)}";

        ShellExecute("dotnet", command);
    }

    private static void ShellExecute(string fileName, string arguments)
    {
        var cmd = new Process();
        cmd.StartInfo.FileName = fileName;
        cmd.StartInfo.Arguments = arguments;
        cmd.Start();
        cmd.WaitForExit();
    }

    private static void RenderDirectoryTemplate(string dirPath, ModuleModel model)
    {
        Log($"Render templates from {dirPath}");
        var searchPatterns = new[] { "*.csproj", "*.cs" };
        var allFiles = searchPatterns
            .SelectMany(searchPattern => Directory.EnumerateFiles(dirPath, searchPattern, SearchOption.AllDirectories));

        foreach (var filePath in allFiles)
        {
            RenderFileTemplate(filePath, model);
        }
    }

    private static void RenderFileTemplate(string filePath, ModuleModel model)
    {
        var text = File.ReadAllText(filePath);
        var template = Template.Parse(text, filePath);

        var result = template.Render(model, member => member.Name);

        File.WriteAllText(filePath, result, Encoding.UTF8);
    }


    private static void CopyFilesRecursively(string sourcePath, string targetPath, bool addDbContext)
    {
        if (!Directory.Exists(sourcePath))
        {
            throw Error($"Source directory doesn't exists: {sourcePath}");
        }

        // TODO: Uncomment this
        // if (Directory.Exists(targetPath))
        // {
        //     throw Error($"Target directory already exists: {targetPath}");
        // }

        Log($"Copy {sourcePath} to {targetPath}");

        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            if (!addDbContext && dirPath.Contains("Engine")) // TODO Make it prettier
            {
                continue;
            }

            var newPath = dirPath.Replace(sourcePath, targetPath);
            Directory.CreateDirectory(newPath);
        }

        foreach (var filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            if (!addDbContext && filePath.Contains("Engine")) // TODO Make it prettier
            {
                continue;
            }

            var newPath = filePath.Replace(sourcePath, targetPath);
            File.Copy(filePath, newPath, true);
        }
    }

    private static string FindUpSolutionFilePath(string startDir)
    {
        var currentDir = Path.GetDirectoryName(startDir);
        while (currentDir != null)
        {
            var solutionFiles = Directory.GetFiles(currentDir, "*.sln", SearchOption.TopDirectoryOnly);
            if (solutionFiles.Length > 0)
            {
                var solutionFile = solutionFiles[0];
                Log($"Solution was found at {solutionFile}");
                return solutionFile;
            }

            currentDir = Path.GetDirectoryName(currentDir);
        }

        throw Error($"Can't find solution directory from: {startDir}");
    }
}
