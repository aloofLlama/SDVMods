using HarvestHelper.Helpers;
using HarvestHelper.TooltipSections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Wrappers;
using SDVCommon.Tooltip;
using StardewModdingAPI;
using StardewValley;



namespace HarvestHelper.Services
{
    public static class TooltipBuilder
    {
        public static List<TooltipElement> BuildTooltip(HarvestInfo harvest, StardewValley.Object obj)
        {
            var list = new List<TooltipElement>();

            list.AddRange(FirstSection.Build(harvest, obj));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => GiftLovesSection.Build(harvest, obj));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => ShipmentSection.Build(harvest, obj));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => CookingSection.Build(harvest));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => SeedmakerSection.Build(harvest, obj));

            //keep inventory at bottom
            TooltipBuildHelper.AddSectionWithSeparator(list, () => InventorySection.Build(harvest, obj));
            return list;
        }


    }
}

