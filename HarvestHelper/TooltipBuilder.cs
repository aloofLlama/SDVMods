using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon.Icons;
using SDVCommon.Models.Wrappers;
using HarvestHelper.TooltipSections;
using HarvestHelper.Helpers;
using SDVCommon.Tooltip;
using SDVCommon.Helpers;



namespace HarvestHelper
{
    public static class TooltipBuilder
    {
        public static List<TooltipElement> BuildTooltip(HarvestInfo harvest, StardewValley.Object obj)
        {
            var list = new List<TooltipElement>();

            list.AddRange(FirstSection.Build(harvest, obj));
            //list.AddRange(PlantGrowthSection.Build(plant));

            ////list.Add(new TooltipElement { IsSeparator = true, PaddingTop = 3, PaddingBottom = 3 });
            //AddSectionWithSeparator(list, () => PlantFeaturesSection.Build(plant));
            //AddSectionWithSeparator(list, () => SeedSourceSection.Build(plant));


            //list.Add(new TooltipElement { IsSeparator = true, PaddingTop = 6, PaddingBottom = 6 });

            return list;
        }


    }
}

