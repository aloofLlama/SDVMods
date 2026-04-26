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
            string plantId = plant.Data.SeedId;
            int owned = InventoryHelper.CountInInventory(plantId);

            list.Add(new TooltipElement
            {
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.Owned),
                    owned)
            });

            return list;
        }
    }
}

