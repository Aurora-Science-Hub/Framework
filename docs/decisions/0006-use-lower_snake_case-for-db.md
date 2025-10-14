# Use lower_snake_case for DB as naming convention

## Status

* Status: **accepted**
* Deciders: [alex1ozr](https://github.com/alex1ozr), [pavelkudriavtsevsw](https://github.com/pavelkudriavtsevsw), [demosfen](https://github.com/Demosfen)
* Date: 26.05.2024
* Technical Story: SPACE-76

## Context and Problem Statement

The common practice is to use naming conventions for databases, tables, columns, and other database objects.
As well, we use PostgreSQL, which is case-insensitive for identifiers, but it converts them to lowercase, we need to choose a naming convention.

## Considered Options

* Use `lower_snake_case`
* Use `UpperCamelCase` as default one for EF Core

## Decision Outcome

Chosen option: "Use `lower_snake_case`", because it is more readable and consistent with the PostgreSQL behavior.
It will not require double-quoting for table/field names in SQL queries.

## Links

* [EFCore.NamingConventions](https://github.com/efcore/EFCore.NamingConventions)
* [PostgreSQL naming conventions](https://dba.stackexchange.com/questions/245610/postgresql-naming-conventions)
