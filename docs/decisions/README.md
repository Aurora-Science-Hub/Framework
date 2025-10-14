# Decisions

This directory contains decision records for SWeather.

For new ADRs, please use [adr-template.md](adr-template.md) as basis.
More information on MADR is available at <https://adr.github.io/madr/>.
General information about architectural decision records is available at <https://adr.github.io/>.

### Prerequisites
Before creating a new ADR, it is recommended to install the `adr-tools`:
```shell
dotnet tool install -g adr
```
If you have previously installed `adr` and want to update to the latest version, use the following command:
```shell
dotnet tool update -g adr
```

### Creating a new ADR
To create a new ADR, use the following command:
```shell
adr new 'Some decision' -p ./docs/decisions
```
