using HarvestHelper.Helpers;
using HarvestHelper.TooltipSections;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDVCommon.GameData;
using SDVCommon.GameData.Dictionaries;
using SDVCommon.Helpers;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Models.Tooltip;
using SDVCommon.Rendering;
using SDVCommon.Services;
using SObject = StardewValley.Object;

namespace HarvestHelper.Services
{
    public static class TooltipBuilder
    {
        private static bool _isInitialized;

        private static List<TooltipElement>? _cachedTooltip;
        private static string? _cachedHarvestQId;
        private static int? _cachedHarvestQuality;


        public static void Initialize()
        {
            if (_isInitialized)
                return;

            Reset();
            _isInitialized = true;
        }

        public static void Reset()
        {
            _cachedTooltip = null;
            _cachedHarvestQId = null;
            _cachedHarvestQuality = null;
        }


        public static void DrawTooltip(SpriteBatch b, SObject obj)
        {
            var elements = GetTooltip(obj);
            if (elements is null || elements.Count == 0)
                return;

            TooltipRenderer.DrawLeftandAboveCursor(b, elements);
        }

        public static List<TooltipElement>? GetTooltip(SObject obj)
        {
            string qId = obj.QualifiedItemId;
            int quality = obj.Quality;

            bool needsRebuild =
                _cachedTooltip == null ||
                qId != _cachedHarvestQId ||
                quality != _cachedHarvestQuality;

            if (!needsRebuild)
                return _cachedTooltip;

            _cachedTooltip = BuildTooltip(obj);
            TooltipRenderer.InvalidateSize(_cachedTooltip);
            _cachedHarvestQId = qId;
            _cachedHarvestQuality = quality;

            return _cachedTooltip;
        }

        public static List<TooltipElement> BuildTooltip(SObject obj)
        {
            var list = new List<TooltipElement>();

            string qId = obj.QualifiedItemId;
            var harvest = Harvest.GetHarvestInfo(qId);

            if (harvest == null)
                return list;

            list.AddRange(FirstSection.Build(harvest, obj));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => InventorySection.Build(harvest, obj));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => GiftLovesSection.Build(obj));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => ShipmentSection.Build(harvest));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => CookingSection.BuildSpecific(harvest));
             TooltipBuildHelper.AddSectionWithSeparator(list, () => CookingSection.BuildGeneric(harvest, obj));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => SeedmakerSection.Build(harvest, obj));
            return list;
        }

    }
}

