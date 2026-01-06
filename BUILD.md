# Building CAN Crops Mod

This document explains how to build the CAN Crops mod for Vintage Story.

## Prerequisites

1. **.NET 8.0 SDK** - Required for building the project
2. **Vintage Story** - Install the game to get the required DLLs
3. **VINTAGE_STORY environment variable** - Point to your Vintage Story installation directory

## Required Dependencies

The following DLLs from Vintage Story are **required** to build:
- `VintagestoryAPI.dll`
- `VintagestoryLib.dll`
- `VSSurvivalMod.dll`
- `VSEssentials.dll`
- `VSCreativeMod.dll`
- `0Harmony.dll`
- `Newtonsoft.Json.dll`
- `protobuf-net.dll`
- `cairo-sharp.dll`

These are referenced from `$(VINTAGE_STORY)` environment variable path.

## Optional Mod Compatibility

The mod includes compatibility layers for other optional mods. These are **not required** to build:

- **FarmlandDropsSoil** - Compatibility with FarmlandDropsSoil mod
- **PrimitiveSurvival** - Compatibility with PrimitiveSurvival mod
- **XSkills & XLib** - Compatibility with XSkills mod

### Building Without Optional Mods

The project is configured to build successfully **without** these optional mod DLLs. The compatibility code will be automatically excluded from compilation if the DLLs are not found.

Simply ensure the `VINTAGE_STORY` environment variable is set and run:

```bash
# Linux/Mac
export VINTAGE_STORY="/path/to/vintagestory"
cd cancrops
bash build.sh

# Windows (PowerShell)
$env:VINTAGE_STORY = "C:\Path\To\Vintagestory"
cd cancrops
.\build.ps1
```

### Building With Optional Mods

If you want to include compatibility for optional mods:

1. Create a `libs` directory in the repository root:
   ```
   CAN_crops/
   ├── libs/
   │   ├── FarmlandDropsSoil.dll (optional)
   │   ├── primitivesurvival.dll (optional)
   │   ├── xlib.dll (optional)
   │   └── xskills.dll (optional)
   ├── cancrops/
   └── ...
   ```

2. Copy the mod DLLs to the `libs` folder

3. Build normally - the compatibility code will be automatically included

## Build Process

The build uses Cake Build:

1. **Validate JSON** - Checks all JSON configuration files are valid
2. **Clean** - Removes previous build artifacts
3. **Publish** - Compiles the C# code
4. **Package** - Creates a `.zip` file in the `Releases` folder

## Troubleshooting

### "Could not locate assembly" warnings

If you see warnings like:
```
warning MSB3245: Could not resolve this reference. Could not locate the assembly "FarmlandDropsSoil"
```

These are **normal** if you don't have the optional mod DLLs. The build will still succeed, and the compatibility code will be excluded.

### "The type or namespace name could not be found" errors

If you see actual **errors** (not warnings) about missing types, ensure:
1. The `VINTAGE_STORY` environment variable is set correctly
2. All required Vintage Story DLLs exist in the game installation
3. You're using .NET 8.0 SDK

### Build succeeds but mod doesn't work

The mod compatibility features will only work if:
1. The respective optional mods are installed in Vintage Story
2. The compatibility DLLs were present during build

If you build without the optional DLLs, the mod will work fine but won't have compatibility features for those mods.

## Development

When developing compatibility features for optional mods:

1. The `ShouldLoad()` method in each compat class checks if the mod is enabled
2. Compatibility code only loads if the target mod is present at runtime
3. Build-time exclusion prevents compilation errors when DLLs are missing

## See Also

- [README.md](README.md) - General mod documentation
- [ARCHITECTURE.md](ARCHITECTURE.md) - Technical architecture details
