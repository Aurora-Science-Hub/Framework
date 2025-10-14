using System.CommandLine;
using AuroraScienceHub.Framework.ModuleCreator;

// More info about the System.CommandLine library:
// https://learn.microsoft.com/en-us/dotnet/standard/commandline/get-started-tutorial

var rootCommand = new RootCommand("Welcome to the Module Creator!");

var newCommand = new Command("new", "Create new...");
rootCommand.AddCommand(newCommand);

var addDbContextOption = new Option<bool>(
    name: "--add-db-context",
    description: "Specifies whether to add a database context to the module",
    getDefaultValue: () => false);
var nameArgument = new Argument<string> { Name = "name", Description = "The name of the new module", };

var moduleCommand = new Command("module", "Create a new module") { nameArgument, addDbContextOption, };
newCommand.AddCommand(moduleCommand);

moduleCommand.SetHandler(ModuleHelper.CreateNew, nameArgument, addDbContextOption);

return await rootCommand.InvokeAsync(args).ConfigureAwait(false);
