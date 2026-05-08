using PlantingDay.Helpers;
using PlantingDay.Models.Wrappers;
using Microsoft.Xna.Framework;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Wrappers;
using SDVCommon.Tooltip;
using StardewModdingAPI;
using StardewValley;
using System.Xml.Linq;


namespace PlantingDay.TooltipSections
{
    public static class InventorySection
    {
        public static List<TooltipElement> Build(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            string seedId = plant.Data.SeedId;
            string harvestId = plant.Data.HarvestId;

            //ModEntry.Instance.Monitor.Log(
            //    $"[InventorySection] SeedId={seedId} | HarvestId={harvestId}",
            //    LogLevel.Debug);

            int owned = InventoryHelper.CountOwned(seedId);

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

