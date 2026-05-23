# Text Upgrades UI Scale

Text Upgrades UI Scale keeps long modded upgrade lists readable by dynamically scaling and spacing the upgrade text when too many upgrades are active.

Useful for heavily modded lobbies where the vanilla upgrade UI overflows the screen.

![Preview](https://raw.githubusercontent.com/DarkSpider90/REPO-TextUpgradesUIScale/main/images/preview.png)

## Config

The config file is generated as `BepInEx/config/DarkSpider90.TextUpgradesUIScale.cfg`.

- `StartShrinkingAfterUpgradesCount`: after how many active upgrades the text starts shrinking. Default: `10`.
- `ShrinkEveryUpgrades`: shrink once per this many additional upgrades. Default: `2`.
- `ShrinkStep`: how much scale is removed per step. Default: `0.07`.
- `LineSpacing`: extra vertical spacing between rows while scaled. Default: `-4`.
- `MaxTextWidthBeforeShrinking`: shrink text if upgrade names become wider than this value. Default: `0`, which disables width-based shrinking.

Scale values use `1` as the original game size. For example, `0.5` is half size.

## Installation

### Mod Manager

Install with a Thunderstore-compatible mod manager.

### Manual

1. Install BepInEx.
2. Place `TextUpgradesUIScale.dll` into:

`BepInEx/plugins/TextUpgradesUIScale/`
