# Aurora Science Hub Framework

This repository contains the framework for Aurora Science Hub applications, including common libraries, tools,
and configurations to streamline development and ensure consistency across projects.

## Code style
The solution uses [EditorConfig](.editorconfig) nested from [Azure SDK .NET](https://github.com/Azure/azure-sdk-for-net/blob/main/.editorconfig) to maintain a consistent code style.

- In order to enforce the code style, it is recommended to execute the `dotnet format` command before pushing the changes.
- It is also possible to use the `dotnet format --verify-no-changes` command to check if the code style is consistent.
