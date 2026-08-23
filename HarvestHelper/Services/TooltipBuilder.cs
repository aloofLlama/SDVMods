using HarvestHelper.Helpers;
using HarvestHelper.TooltipSections;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDVCommon.Helpers;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.GameData.Dictionaries;
using SDVCommon.Models.Tooltip;
using SDVCommon.Rendering;
using SObject = StardewValley.Object;

namespace HarvestHelper.Services
{
    public static class TooltipBuilder
    {
        private static bool _isInitialized;

        private static List<TooltipElement>? _cachedTooltip;
        private static string? _cachedHarvestQId;

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

            bool needsRebuild =
                _cachedTooltip == null ||
                qId != _cachedHarvestQId;

            if (!needsRebuild)
                return _cachedTooltip;

            _cachedTooltip = BuildTooltip(obj);
            TooltipRenderer.InvalidateSize(_cachedTooltip);
            _cachedHarvestQId = qId;

            return _cachedTooltip;
        }

        public static List<TooltipElement> BuildTooltip(SObject obj)
        {
            var list = new List<TooltipElement>();

            string qId = obj.QualifiedItemId;
            var harvest = Harvest.GetHarvestInfo(qId);

            //string tempuQid = obj.ItemId;
            //var harvest = Harvest.LookupFromKey(tempuQid);

            if (harvest == null)
                return list;

            list.AddRange(FirstSection.Build(harvest, obj));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => InventorySection.Build(harvest, obj));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => GiftLovesSection.Build(obj));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => ShipmentSection.Build(harvest));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => CookingSection.BuildSpecific(harvest));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => CookingSection.BuildGeneric(harvest));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => SeedmakerSection.Build(harvest, obj));
            return list;
        }

    }
}

