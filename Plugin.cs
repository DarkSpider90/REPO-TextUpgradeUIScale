using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using TextUpgradesUIScale.Patches;

namespace TextUpgradesUIScale
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string ModGuid = "DarkSpider90.TextUpgradesUIScale";
        private const string ModName = "TextUpgradesUIScale";
        private const string ModVersion = "1.2.0";

        private readonly Harmony harmony = new Harmony(ModGuid);

        internal static ManualLogSource Log { get; private set; }

        internal static ConfigEntry<bool> ModEnabled { get; private set; }
        internal static ConfigEntry<int> StartShrinkingAfterUpgradesCount { get; private set; }
        internal static ConfigEntry<int> ShrinkEveryUpgrades { get; private set; }
        internal static ConfigEntry<float> ShrinkStep { get; private set; }
        internal static ConfigEntry<float> LineSpacing { get; private set; }
        internal static ConfigEntry<float> MaxTextWidthBeforeShrinking { get; private set; }

        private void Awake()
        {
            Log = Logger;
            BindConfig();

            Log.LogInfo("Loaded TextUpgradesUIScale 1.2.0 for R.E.P.O. v0.4.0 UI.");
            harmony.PatchAll(typeof(StatsUIPatch));
        }

        private void BindConfig()
        {
            ModEnabled = Config.Bind("General", "EnableMod", true, "Enable or disable the mod.");

            StartShrinkingAfterUpgradesCount = Config.Bind("Text fitting", "StartShrinkingAfterUpgradesCount", 10, new ConfigDescription("Start shrinking text after this many active upgrades.", new AcceptableValueRange<int>(0, 100)));
            ShrinkEveryUpgrades = Config.Bind("Text fitting", "ShrinkEveryUpgrades", 2, new ConfigDescription("Shrink text once per this many additional upgrades.", new AcceptableValueRange<int>(1, 20)));
            ShrinkStep = Config.Bind("Text fitting", "ShrinkStep", 0.07f, new ConfigDescription("Text scale removed per shrink step. 0.07 means 7%.", new AcceptableValueRange<float>(0.01f, 0.5f)));

            LineSpacing = Config.Bind("Layout", "LineSpacing", -4f, new ConfigDescription("Extra vertical spacing between upgrade rows while scaled. Negative values make the list tighter.", new AcceptableValueRange<float>(-30f, 30f)));
            MaxTextWidthBeforeShrinking = Config.Bind("Layout", "MaxTextWidthBeforeShrinking", 0f, new ConfigDescription("Shrink text if the upgrade names become wider than this value. Set to 0 to disable width-based shrinking.", new AcceptableValueRange<float>(0f, 2000f)));
        }
    }
}
