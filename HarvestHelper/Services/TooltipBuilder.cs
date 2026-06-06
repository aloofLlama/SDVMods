using HarvestHelper.Helpers;
using HarvestHelper.TooltipSections;
using SDVCommon.Helpers;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Models.Tooltip;
using SDVData;
using StardewModdingAPI;

namespace HarvestHelper.Services
{
    public static class TooltipBuilder
    {
        public static List<TooltipElement> BuildTooltip(HarvestInfo harvest, StardewValley.Object obj)
        {
            var list = new List<TooltipElement>();

            list.AddRange(FirstSection.Build(harvest, obj));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => InventorySection.Build(harvest, obj));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => GiftLovesSection.Build(obj));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => ShipmentSection.Build(harvest, obj));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => CookingSection.Build(harvest));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => SeedmakerSection.Build(harvest, obj));
            return list;
        }

    }
}

