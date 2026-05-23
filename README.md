# Text Upgrade UI Scale

Useful for heavily modded lobbies where the vanilla upgrade UI overflows the screen.
Text Upgrade UI Scale keeps long modded upgrade lists readable by dynamically scaling and spacing the upgrade text when too many upgrades are active.

## Config

The config file is generated as `BepInEx/config/DarkSpider90.TextUpgradesUIScale.cfg`.

- `StartShrinkingAfterUpgradesCount`: after how many active upgrades the text starts shrinking. Default: `10`.
- `ShrinkEveryUpgrades`: shrink once per this many additional upgrades. Default: `2`.
- `ShrinkStep`: how much scale is removed per step. Default: `0.1`.
- `LineSpacing`: extra vertical spacing between rows while scaled. Default: `-4`.
- `MaxTextWidthBeforeShrinking`: shrink text if upgrade names become wider than this value. Default: `0`, which disables width-based shrinking.

Scale values use `1` as the original game size. For example, `0.5` is half size.

## Installation

### Mod Manager

Install with a Thunderstore-compatible mod manager.

### Manual

1. Install BepInEx.
2. Place `TextUpgradeUIScale.dll` into:

`BepInEx/plugins/TextUpgradeUIScale/`
