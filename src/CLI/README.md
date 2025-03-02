# CnSharp.Updater.CLI

This is a packaging & deployment CLI of `SharpUpdater`.

## Installation
```
dotnet tool install --global SharpUpdater.CLI
```

## Root Command
```
su
```

## Commands
### `global`

Sets the global settings.

**Options:**
- `--source`, `-s` : Set the global SharpUpdater.Server source URL (Required)

```
su global -s http://your.server
```

### `RemoveSource`

Removes the global source.

```
su RemoveSource
```

### `init`

Generates a manifest file in the current directory.

```
su init
```

### `ignore`

Generates an ignore file in the current directory.

```
su ignore
```

### `pack`

Packs the project.

**Options:**
- `--source`, `-s` : Specify the SharpUpdater.Server source URL (Optional, follows global source if not inputed)
- `--project`, `-p` : Specify the project directory
- `--output`, `-o` : Specify the output directory (Default: `bin\SharpUpdater\`)
- `--version`, `-v` : Specify the package version
- `--MinimumVersion`, `-mv` : Specify the minimum version that must be updated
- `--ReleaseNotes`, `-rn` : Input release notes
- `--no-build` : Skip build

```
su pack -v 1.0.0
```

### `push`

Pushes the .sp package to SharpUpdater.Server.

**Options:**
- `--package`, `-p` : Specify the .sp file path (Required)
- `--source`, `-s` : Specify the SharpUpdater.Server source URL (Optional, follows global source if not inputed)
- `--apikey`, `-k` : Specify the ApiKey of SharpUpdater.Server (Required)

```
su push -p some.sp -k YOUR_KEY
```