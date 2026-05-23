using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace TextUpgradesUIScale.Patches
{
    [HarmonyPatch(typeof(StatsUI), "Fetch")]
    public static class StatsUIPatch
    {
        private const float TechnicalMinimumScale = 0.05f;

        private static readonly FieldInfo TextField = AccessTools.Field(typeof(StatsUI), "Text");
        private static readonly FieldInfo NumbersTextField = AccessTools.Field(typeof(StatsUI), "textNumbers");
        private static readonly FieldInfo PlayerUpgradesField = AccessTools.Field(typeof(StatsUI), "playerUpgrades");

        private static TextMeshProUGUI _cachedMainText;
        private static TextMeshProUGUI _cachedNumbersText;
        private static float _defaultFontSize;
        private static float _defaultNumbersFontSize;
        private static bool _defaultAutoSizing;
        private static bool _defaultNumbersAutoSizing;
        private static bool _defaultWordWrapping;
        private static bool _defaultNumbersWordWrapping;
        private static TextOverflowModes _defaultOverflowMode;
        private static TextOverflowModes _defaultNumbersOverflowMode;
        private static VerticalAlignmentOptions _defaultVerticalAlignment;
        private static VerticalAlignmentOptions _defaultNumbersVerticalAlignment;
        private static float _defaultLineSpacing;
        private static float _defaultNumbersLineSpacing;
        private static bool _defaultFontSizeInitialized;
        private static bool _loggedMissingFields;
        private static bool _loggedException;

        public static void Postfix(StatsUI __instance)
        {
            try
            {
                var mainText = TextField == null ? null : TextField.GetValue(__instance) as TextMeshProUGUI;
                var numbersText = NumbersTextField == null ? null : NumbersTextField.GetValue(__instance) as TextMeshProUGUI;
                var playerUpgrades = PlayerUpgradesField == null ? null : PlayerUpgradesField.GetValue(__instance) as Dictionary<string, int>;

                if (mainText == null || numbersText == null || playerUpgrades == null)
                {
                    LogMissingFieldsOnce();
                    return;
                }

                CaptureDefaultsIfNeeded(mainText, numbersText);

                if (!Plugin.ModEnabled.Value)
                {
                    RestoreTextColumns(mainText, numbersText);
                    return;
                }

                int activeUpgradesCount = CountActiveUpgrades(playerUpgrades);
                float scale = GetScaleForUpgradeCount(activeUpgradesCount);
                scale = ApplyWidthLimit(mainText, scale);

                if (scale >= 0.999f)
                {
                    RestoreTextColumns(mainText, numbersText);
                    return;
                }

                ConfigureTextColumn(mainText, _defaultLineSpacing);
                ConfigureTextColumn(numbersText, _defaultNumbersLineSpacing);

                mainText.fontSize = _defaultFontSize * scale;
                numbersText.fontSize = _defaultNumbersFontSize * scale;
            }
            catch (Exception ex)
            {
                if (!_loggedException)
                {
                    _loggedException = true;
                    if (Plugin.Log != null)
                    {
                        Plugin.Log.LogWarning("[TextUpgradesUIScale] Failed to resize StatsUI text: " + ex.Message);
                    }
                }
            }
        }

        private static void CaptureDefaultsIfNeeded(TextMeshProUGUI mainText, TextMeshProUGUI numbersText)
        {
            if (_defaultFontSizeInitialized && _cachedMainText == mainText && _cachedNumbersText == numbersText)
            {
                return;
            }

            _cachedMainText = mainText;
            _cachedNumbersText = numbersText;
            _defaultFontSize = mainText.fontSize;
            _defaultNumbersFontSize = numbersText.fontSize;
            _defaultAutoSizing = mainText.enableAutoSizing;
            _defaultNumbersAutoSizing = numbersText.enableAutoSizing;
            _defaultWordWrapping = mainText.enableWordWrapping;
            _defaultNumbersWordWrapping = numbersText.enableWordWrapping;
            _defaultOverflowMode = mainText.overflowMode;
            _defaultNumbersOverflowMode = numbersText.overflowMode;
            _defaultVerticalAlignment = mainText.verticalAlignment;
            _defaultNumbersVerticalAlignment = numbersText.verticalAlignment;
            _defaultLineSpacing = mainText.lineSpacing;
            _defaultNumbersLineSpacing = numbersText.lineSpacing;
            _defaultFontSizeInitialized = true;
        }

        private static int CountActiveUpgrades(Dictionary<string, int> playerUpgrades)
        {
            int count = 0;
            foreach (var upgrade in playerUpgrades)
            {
                if (upgrade.Value > 0)
                {
                    count++;
                }
            }

            return count;
        }

        private static void ConfigureTextColumn(TextMeshProUGUI text, float defaultLineSpacing)
        {
            text.enableAutoSizing = false;
            text.enableWordWrapping = false;
            text.overflowMode = TextOverflowModes.Overflow;
            text.verticalAlignment = VerticalAlignmentOptions.Top;
            text.lineSpacing = defaultLineSpacing + Plugin.LineSpacing.Value;
        }

        private static void RestoreTextColumns(TextMeshProUGUI mainText, TextMeshProUGUI numbersText)
        {
            RestoreTextColumn(mainText, _defaultFontSize, _defaultAutoSizing, _defaultWordWrapping, _defaultOverflowMode, _defaultVerticalAlignment, _defaultLineSpacing);
            RestoreTextColumn(numbersText, _defaultNumbersFontSize, _defaultNumbersAutoSizing, _defaultNumbersWordWrapping, _defaultNumbersOverflowMode, _defaultNumbersVerticalAlignment, _defaultNumbersLineSpacing);
        }

        private static void RestoreTextColumn(TextMeshProUGUI text, float fontSize, bool autoSizing, bool wordWrapping, TextOverflowModes overflowMode, VerticalAlignmentOptions verticalAlignment, float lineSpacing)
        {
            text.fontSize = fontSize;
            text.enableAutoSizing = autoSizing;
            text.enableWordWrapping = wordWrapping;
            text.overflowMode = overflowMode;
            text.verticalAlignment = verticalAlignment;
            text.lineSpacing = lineSpacing;
        }

        private static float GetScaleForUpgradeCount(int activeUpgradesCount)
        {
            int startShrinkingAfter = Mathf.Max(0, Plugin.StartShrinkingAfterUpgradesCount.Value);
            if (activeUpgradesCount <= startShrinkingAfter)
            {
                return 1f;
            }

            int shrinkEveryUpgrades = Mathf.Max(1, Plugin.ShrinkEveryUpgrades.Value);
            float shrinkStep = Mathf.Clamp(Plugin.ShrinkStep.Value, 0.01f, 0.5f);
            int extraUpgrades = Mathf.Max(0, activeUpgradesCount - startShrinkingAfter);
            int steps = Mathf.CeilToInt(extraUpgrades / (float)shrinkEveryUpgrades);

            return ClampScale(1f - (steps * shrinkStep));
        }

        private static float ApplyWidthLimit(TextMeshProUGUI mainText, float currentScale)
        {
            float maxTextWidth = Plugin.MaxTextWidthBeforeShrinking.Value;
            if (maxTextWidth <= 1f)
            {
                return currentScale;
            }

            ConfigureTextColumn(mainText, _defaultLineSpacing);

            mainText.fontSize = _defaultFontSize * currentScale;
            float currentWidth = mainText.GetPreferredValues(mainText.text, Mathf.Infinity, Mathf.Infinity).x;
            if (currentWidth <= maxTextWidth)
            {
                return currentScale;
            }

            float low = TechnicalMinimumScale;
            float high = currentScale;

            for (int i = 0; i < 8; i++)
            {
                float mid = (low + high) * 0.5f;
                mainText.fontSize = _defaultFontSize * mid;
                float width = mainText.GetPreferredValues(mainText.text, Mathf.Infinity, Mathf.Infinity).x;

                if (width <= maxTextWidth)
                {
                    low = mid;
                }
                else
                {
                    high = mid;
                }
            }

            return ClampScale(low);
        }

        private static float ClampScale(float scale)
        {
            return Mathf.Clamp(scale, TechnicalMinimumScale, 1f);
        }

        private static void LogMissingFieldsOnce()
        {
            if (_loggedMissingFields)
            {
                return;
            }

            _loggedMissingFields = true;
            if (Plugin.Log != null)
            {
                Plugin.Log.LogWarning("[TextUpgradesUIScale] Could not access StatsUI text fields. The game UI may have changed.");
            }
        }
    }
}
