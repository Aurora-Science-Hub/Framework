# ACE and DSCOVR quality flags

## Status

* Status: **accepted**
* Deciders: [Demosfen](https://github.com/Demosfen)
* Date: 13.05.2024
* Documentation Story: SPACE-63

## Context and Problem Statement

ACE spacecraft have different quality flags. The quality flags are used to indicate the quality of the data.
`0` means good data, `1-8` bad data. `9` missing data. But in practice NOAA marks with 1-8 flags bad data records,
while in the real data files the flag 3 can indicate missing data.
Also, data marked with 0 and 1 flags are looks good and similar, while in the real data files they can indicate missing data.
This can lead to confusion when plotting and analysing data from the ACE, because the flags are not consistent with the data in the parsed files.

## Considered Options

* Do not use original ACE data quality flags;
* Calculate the quality flags based on the data in the parsed files and use only data missing values;
* Implement as binary flags if `null` or `missing data value` in ACE and DSCOVR database tables.

## Decision Outcome

Implement as [flag enum](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/enum) if `null` or
`missing data value` in ACE and DSCOVR data files.

| Name               | Flag | Description                                                                 |
|--------------------|------|-----------------------------------------------------------------------------|
| Missing            | 0    | IMF & SW data totally absent. Missing data values in the both source files. |
| Good               | 1    | Full IMF & SW data. No missing data values in the both source files.        |
| MissingBx          | 2    | IMF Bx data is missing.                                                     |
| MissingBy          | 3    | IMF By data is missing.                                                     |
| MissingBz          | 4    | IMF Bz data is missing.                                                     |
| MissingVelocity    | 5    | SW Velocity data is missing.                                                |
| MissingDensity     | 6    | SW Density data is missing.                                                 |
| MissingTemperature | 7    | SW Temperature data is missing.                                             |

Convenient way to analyse the data and plot the data with the quality flags.
