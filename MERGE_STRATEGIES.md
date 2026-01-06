# Seed Merge Strategies

This document explains how the seed merge strategy configuration affects crop breeding in the CAN Crops mod.

## Overview

When breeding two parent crops, their genetic stats must be combined to create the offspring. The `seedMergeStrategy` configuration option controls how this combination works.

## Available Strategies

### 1. "tolower" Strategy (Default)

**Behavior**: The child inherits the **lower value** from the two parents for each stat.

**Example**:
```
Parent 1: Gain=8, Growth=6, Strength=7
Parent 2: Gain=5, Growth=9, Strength=4

Offspring (before mutation):
  Gain = min(8, 5) = 5
  Growth = min(6, 9) = 6
  Strength = min(7, 4) = 4
```

**Use Case**: This strategy makes breeding more challenging and realistic. Players must carefully select parents to avoid losing good stats. It prevents "runaway" breeding where stats only go up.

**Pros**:
- More strategic breeding decisions required
- Harder to achieve perfect crops (end-game goal)
- Encourages maintaining diverse crop populations

**Cons**:
- Can be frustrating if unlucky with inheritance
- Takes longer to improve crop lines

### 2. "mean" Strategy

**Behavior**: The child inherits the **average value** from the two parents for each stat (rounded).

**Example**:
```
Parent 1: Gain=8, Growth=6, Strength=7
Parent 2: Gain=5, Growth=9, Strength=4

Offspring (before mutation):
  Gain = round((8 + 5) / 2) = 7
  Growth = round((6 + 9) / 2) = 8
  Strength = round((7 + 4) / 2) = 6
```

**Use Case**: This strategy is more forgiving and allows gradual improvement through breeding. Good for casual play or if you want faster breeding progress.

**Pros**:
- Gradual stat improvement possible
- Less punishing for poor parent selection
- Faster path to high-stat crops

**Cons**:
- May make breeding too easy
- Reduces strategic depth
- Can lead to stat homogenization

## How It's Configured

In your `cancrops.json` config file:

```json
{
  "seedMergeStrategy": "tolower",
  "seedMergeStrategies": ["tolower", "mean"]
}
```

- `seedMergeStrategy`: The currently active strategy
- `seedMergeStrategies`: List of valid options (for validation)

**To change the strategy**:
1. Open `%appdata%/VintagestoryData/ModConfig/cancrops.json` (Windows)
2. Or `~/.config/VintagestoryData/ModConfig/cancrops.json` (Linux)
3. Change `"seedMergeStrategy"` to `"mean"` or `"tolower"`
4. Restart the game

## Interaction with Mutations

**Important**: The merge strategy is applied BEFORE mutations occur.

**Complete breeding flow**:
1. Select two parent crops based on fertility stat
2. Apply merge strategy to combine parent stats → base offspring stats
3. For each stat, check if mutation occurs (based on mutativity)
4. If mutation: Add or subtract 1 from the stat value
5. Final offspring stats are used for the new crop

**Example with mutations**:
```
Parent 1: Gain=8 (Mutativity=7)
Parent 2: Gain=5 (Mutativity=7)

Step 1 (tolower strategy): Gain = 5
Step 2 (mutation check): Roll for mutation
Step 3 (mutation succeeds): Gain = 5 + 1 = 6

Final offspring: Gain=6
```

## Which Strategy Should You Use?

### Choose "tolower" if you want:
- Traditional challenging breeding mechanics (like AgriCraft)
- Long-term breeding project gameplay
- Strategic importance of parent selection
- Risk/reward in crossbreeding experiments

### Choose "mean" if you want:
- More forgiving breeding system
- Faster crop improvement
- Simplified breeding for casual gameplay
- Focus on other game aspects while breeding on the side

## Technical Implementation

The merge strategy is currently defined in the config but the actual implementation is handled by the genetics system:

**Relevant files**:
- `Config.cs`: Defines the configuration option
- `AgriCombineLogic.cs`: Implements the breeding logic
- `AgriMutationHandler.cs`: Orchestrates the overall breeding process

**Note**: The current implementation in the codebase uses genetic recombination logic with random allele selection from each parent, which is more sophisticated than simple "tolower" or "mean". The config option exists but may need integration work to fully control the breeding behavior.

## Future Enhancements

Potential additional merge strategies:
- **"tohigher"**: Take the higher value (easier breeding)
- **"random"**: Randomly pick parent 1 or parent 2's value
- **"weighted"**: Weight by fertility stat (higher fertility = more likely to pass stats)
- **"genetic"**: Full Mendelian genetics with dominant/recessive rules

## Troubleshooting

**Q: I changed the strategy but breeding still seems the same?**
A: Make sure to restart the game after changing config. The setting is loaded at startup.

**Q: Can I change the strategy mid-game?**
A: Yes, but it only affects future breeding. Existing crops keep their stats.

**Q: Does this affect mutations?**
A: No, mutations are applied after the merge strategy. They're controlled separately by the mutativity stat.

**Q: What happens with invalid strategy names?**
A: The mod will fall back to "tolower" (the default) and log a warning.

## See Also

- [README.md](README.md) - General mod documentation
- [ARCHITECTURE.md](ARCHITECTURE.md) - Technical implementation details
- Config.cs - Source code for configuration options
