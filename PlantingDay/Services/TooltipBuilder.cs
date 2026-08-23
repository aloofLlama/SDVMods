using Microsoft.Xna.Framework.Graphics;
using PlantingDay.ToolTip_Sections;
using PlantingDay.TooltipSections;
using SDVCommon.Helpers;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Models.Builders;
using SDVCommon.Models.Tooltip;
using SDVCommon.Models.Wrappers;
using SDVCommon.Rendering;
using StardewModdingAPI;

namespace PlantingDay.Services
{
    public static class TooltipBuilder
    {
        private static bool _isInitialized;
        private static List<TooltipElement>? _cachedTooltip;
        private static string? _cachedPlantQId;

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
            _cachedPlantQId = null;
        }

        public static void DrawTooltip(SpriteBatch b, StardewValley.Object obj)
        {
            var elements = GetTooltip(obj);
            if (elements is null || elements.Count == 0)
                return;

            TooltipRenderer.DrawLeftandAboveCursor(b, elements);
        }

        public static List<TooltipElement>? GetTooltip(StardewValley.Object obj)
        {
            string qId = obj.QualifiedItemId;

            bool needsRebuild =
                _cachedTooltip == null ||
                qId != _cachedPlantQId;

            if (!needsRebuild)
                return _cachedTooltip;

            _cachedTooltip = BuildTooltip(obj);
            TooltipRenderer.InvalidateSize(_cachedTooltip);
            _cachedPlantQId = qId;

            return _cachedTooltip;
        }

        public static List<TooltipElement> BuildTooltip(StardewValley.Object obj)
        {
            var list = new List<TooltipElement>();

            string qId = obj.QualifiedItemId;
            //var plant = PlantInfoBuilder.LookupFromKey(qId);

            string tempuQid = obj.ItemId;
            var plant = PlantInfoBuilder.LookupFromKey(tempuQid);

            if (plant == null)
                return list;

            TooltipBuildHelper.AddIfNotNull(list, SeasonSection.Build(plant));
            list.AddRange(PlantGrowthSection.Build(plant));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => PlantFeaturesSection.Build(plant));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => SeedSourceSection.Build(plant));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => HarvestEconomicsSection.Build(plant));
            
            //Keep inventory at bottom
            TooltipBuildHelper.AddSectionWithSeparator(list, () => InventorySection.Build(plant));

            return list;
        }


    }
}



/*

            //----------------
            // How many I have
            //----------------


            //----------------
            // Harvest value
            //----------------

            //TODO: Try this command that UI Info Suite Alt 2 uses:    return GetHarvest(item)?.sellToStorePrice() ?? 0;
            int harvestBV = plant.Data.HarvestPrice; //Base value of harvest items
            //ModEntry.Instance.Monitor.Log($"BV: {harvestBV}", LogLevel.Info);
            int goldStarHarvest = (int)Math.Floor(1.5 * harvestBV); //Value of gold star quality harvest items


            list.Add(new TooltipElement
            {
                Icon = TooltipIcons.GoldStar,
                Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.PriceRange),
                        harvestBV,
                        goldStarHarvest),
                TextColor = TooltipColors.Normal
            });

            return list;



        }

    }

}
        */

