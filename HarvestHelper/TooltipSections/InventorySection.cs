using HarvestHelper.Helpers;
using Microsoft.Xna.Framework;
using SDVCommon;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Wrappers;
using SDVCommon.Models.Runtime;
using StardewModdingAPI;
using StardewValley;
using System.Xml.Linq;


namespace HarvestHelper.TooltipSections
{
    public static class InventorySection
    {
        public static List<TooltipElement> Build(HarvestInfo harvest, StardewValley.Object obj)
        {
            var list = new List<TooltipElement>();
            string harvestId = harvest.Data.HarvestId;
            int owned = InventoryHelper.CountOwned(harvestId);

            list.Add(new TooltipElement
            {
                Icon = harvest.Runtime.HarvestIcon,
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.Owned),
                    owned)
            });

            return list;
        }
    }
}

