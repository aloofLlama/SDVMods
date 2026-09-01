using PlantingDay.Helpers;
using SDVCommon.GameData.Dictionaries;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Tooltip;
using SDVCommon.Models.Wrappers;

namespace PlantingDay.ToolTip_Sections
{
    internal class HarvestEconomicsSection
    {
        public static List<TooltipElement> Build(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            if (plant.Data.DaysToProduce <= 0)
                return list;

            var harvestId = plant.Data.HarvestId;
            var harvest = Harvest.GetHarvestInfo(harvestId);

            if (harvest == null)
                return list;

            int harvestPrice = REFACTOREconomicsHelper.GetHarvestSellPriceFromSeed(plant.Data.SeedId);

            list.Add(new TooltipElement
            {
                Icon = IconRegistry.GetIcon(harvest.HarvestQId),
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.BasicPrice),
                    harvestPrice)
            });

            return list;
        }
    }
}





