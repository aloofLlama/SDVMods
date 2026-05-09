using PlantingDay.Helpers;
using SDVCommon.GameData;
using SDVCommon.Models.Tooltip;
using SDVCommon.Models.Wrappers;


namespace PlantingDay.TooltipSections
{
    public static class InventorySection
    {
        public static List<TooltipElement> Build(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            string seedId = plant.Data.SeedId;
            string harvestId = plant.Data.HarvestId;

            int owned = Inventory.CountOwned(seedId);

            list.Add(new TooltipElement
            {
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.Owned), owned)
            });

            //int planted = PlantedHelper.CountPlanted(harvestId);

            //list.Add(new TooltipElement
            //{
            //    Text = $"planted x{planted}"
            //});

            return list;
        }


    }
}

