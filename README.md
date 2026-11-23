# SunamoFilesIndex

## Short description

Indexování souborů a složek a jejich získávání podle podmínek.


Index files/folders and getting them with conditions

## Overview

SunamoFilesIndex is part of the Sunamo package ecosystem, providing modular, platform-independent utilities for .NET development.

## Main Components

### Key Classes

- **CheckBoxDataShared**
- **TWithSize**
- **FileIndex**
- **FileItem**
- **FolderItem**

### Key Methods

- `GetFoldersWithName()`
- `FindAllFilesWithName()`
- `AddFolderRecursively()`
- `GetRelativeFolder()`
- `Nuke()`

## Installation

```bash
dotnet add package SunamoFilesIndex
```

## Dependencies

- **Microsoft.Extensions.Logging.Abstractions** (v9.0.3)

## Package Information

- **Package Name**: SunamoFilesIndex
- **Version**: 25.3.29.1
- **Target Framework**: net9.0
- **Category**: Platform-Independent NuGet Package
- **Source Files**: 11

## Related Packages

This package is part of the Sunamo package ecosystem. For more information about related packages, visit the main repository.

## License

See the repository root for license information.

<!-- sunamo:test-coverage:start -->
## Test coverage

| Metric | Value |
|---|---|
| Method | static |
| Test projects | 1 |
| Public types referenced by tests | 0/5 (0.0 %) |
| Public methods referenced by tests | 0/11 (0.0 %) |
| Line coverage | n/a |
| Branch coverage | n/a |
| Generated (UTC) | 2026-11-22 21:24 |

<!-- sunamo:test-coverage:data {"package":"SunamoFilesIndex","container":"pinp","testProjects":1,"publicTypes":5,"typesReferencedByTests":0,"publicMethods":11,"methodsReferencedByTests":0,"lineRate":null,"branchRate":null,"method":"static","generatedUtc":"2026-11-22 21:24"} -->
<!-- sunamo:test-coverage:end -->
