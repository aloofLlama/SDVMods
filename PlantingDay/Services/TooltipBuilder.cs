using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay.Helpers;
using SDVCommon.Icons;
using PlantingDay.ToolTip_Sections;
using PlantingDay.Models.Runtime;
using PlantingDay.Models.Wrappers;
using SDVCommon.Tooltip;
using SDVCommon.Helpers;



namespace PlantingDay.Services
{
    public static class TooltipBuilder
    {
        public static List<TooltipElement> BuildTooltip(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            TooltipBuildHelper.AddIfNotNull(list, SeasonSection.Build(plant));
            list.AddRange(PlantGrowthSection.Build(plant));

            //list.Add(new TooltipElement { IsSeparator = true, PaddingTop = 3, PaddingBottom = 3 });
            TooltipBuildHelper.AddSectionWithSeparator(list, () => PlantFeaturesSection.Build(plant));
            TooltipBuildHelper.AddSectionWithSeparator(list, () => SeedSourceSection.Build(plant));

            ////NOT YET REFACTORED BELOW HERE. PULLING EACH SECTION TO OWN FILE

            list.Add(new TooltipElement { IsSeparator = true, PaddingTop = 6, PaddingBottom = 6 });

            //list.AddRange(GetEconomicsTooltip(plant));

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

