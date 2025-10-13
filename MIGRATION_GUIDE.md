# Migration Guide: Copying Files from SWeather-core

This guide explains how to copy the framework components from the SWeather-core repository to this SWeather-framework repository.

## Prerequisites

- Access to the SWeather-core repository (https://github.com/Aurora-Science-Hub/SWeather-core)
- Git command line tools installed

## Files to Copy

### 1. Framework Source Code
Copy the entire `/src/Framework` directory from SWeather-core:
```bash
# From SWeather-core repository
cp -r /path/to/SWeather-core/src/Framework/* /path/to/SWeather-framework/src/Framework/
```

### 2. Framework Unit Tests
Copy the entire `/tests/Framework/UnitTests` directory from SWeather-core:
```bash
# From SWeather-core repository
cp -r /path/to/SWeather-core/tests/Framework/UnitTests/* /path/to/SWeather-framework/tests/Framework/UnitTests/
```

### 3. Root Directory Files
Copy all files from the root directory of SWeather-core, **except** `build.*` files:
```bash
# From SWeather-core repository root
# Copy all files except build.* files
cd /path/to/SWeather-core
find . -maxdepth 1 -type f ! -name 'build.*' -exec cp {} /path/to/SWeather-framework/ \;
```

Or manually copy files like:
- Solution file (*.sln)
- Directory.Build.props
- Directory.Build.targets
- global.json
- nuget.config
- Other configuration files

**Note**: Do NOT copy:
- build.cmd
- build.ps1
- build.sh
- Any other build.* files

## After Copying

1. Review the copied files to ensure everything is correct
2. Update any references or paths that might be specific to SWeather-core
3. Commit the changes:
   ```bash
   git add .
   git commit -m "Copy framework projects from SWeather-core"
   git push
   ```

## Directory Structure

After copying, the repository should have this structure:
```
SWeather-framework/
├── src/
│   └── Framework/
│       └── [Framework project files]
├── tests/
│   └── Framework/
│       └── UnitTests/
│           └── [Unit test files]
├── .gitignore
├── LICENSE
├── README.md
└── [Other root configuration files]
```
