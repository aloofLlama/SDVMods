
using PlantingDay.Helpers;
using SDVCommon.Models.Tooltip;
using SDVCommon.Models.Wrappers;
using SDVCommon.Models.Builders;
using SDVCommon.Helpers;

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
            var harvest = HarvestInfoBuilder.LookupFromKey(harvestId);

            if (harvest == null)
                return list;

            int harvestPrice = EconomicsHelper.GetHarvestSellPriceFromSeed(plant.Data.SeedId);

            list.Add(new TooltipElement
            {
                Icon = IconRegistry.GetIcon(harvest.HarvestId),
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.BasicPrice),
                    harvestPrice)
            });

            return list;
        }
    }
}





