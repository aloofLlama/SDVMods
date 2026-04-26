
using PlantingDay.Helpers;
using PlantingDay.Models.Runtime;
using PlantingDay.Models.Wrappers;
using SDVCommon;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Tooltip;
using SDVData;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlantingDay.ToolTip_Sections
{
    internal class HarvestEconomicsSection
    {
        public static List<TooltipElement> Build(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            if (plant.Data.DaysToProduce <= 0)
                return list;

            // Look up the harvest info
            var harvestId = plant.Data.HarvestId;
            var harvestInfo = HarvestInfoBuilder.LookupFromKey(harvestId);

            if (harvestInfo == null)
                return list; // no harvest info found

            var harvestIcon = harvestInfo.Runtime.HarvestIcon;
            int harvestPrice = harvestInfo.Data.Price;

            list.Add(new TooltipElement
            {
                Icon = harvestIcon,
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.BasicPrice),
                    harvestPrice)
            });

            return list;
        }
    }
}





