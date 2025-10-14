# NuGet packages

The solution uses [GitHub Packages](https://github.com/features/packages) to store and manage NuGet packages.

It is necessary to set up the NuGet source to be able to publish and consume the private packages.

## Prerequisites
### PAT token generation
1. Go to [GitHub settings](https://github.com/settings/profile).
2. Click on the `Developer settings` tab.
3. Click on the `Personal access tokens` tab and select the `Tokens (classic)` section.
4. Click on the `Generate new token` button for `Tokens (classic)`.
5. Enter a token description and select the `read:packages` and `write:packages` scopes.

### NuGet source configuration
It's possible to configure the NuGet source in the `NuGet.config` file (as it is already done in the current [Solution](../../NuGet.Config)).

The one thing that should be configured then is the `UserName` and `api-key` values.
It can be done via Visual Studio or Rider UI or by executing the following command:
```shell
dotnet nuget add source https://nuget.pkg.github.com/Aurora-Science-Hub/index.json --name AuroraScienceHub --username <GITHUB_USERNAME> --password <PAT_TOKEN>
```

## Publishing a package (manually)
Execute the following command
```shell
dotnet pack --configuration Release
dotnet nuget push "your npkg file" --source "AuroraScienceHub" --api-key "your api key"
```

## Articles
- [Package source mapping](https://learn.microsoft.com/en-us/nuget/consume-packages/package-source-mapping)
- [Consuming a NuGet package from GitHub Packages](https://samlearnsazure.blog/2021/08/08/consuming-a-nuget-package-from-github-packages/)
- [Publishing a NuGet package using GitHub and GitHub Actions](https://www.meziantou.net/publishing-a-nuget-package-following-best-practices-using-github.htm)

