using PlantingDay.Helpers;
using SDVCommon.Icons;
using StardewValley.GameData.Crops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDVData;
using PlantingDay.Models.Runtime;
using PlantingDay.Models.Wrappers;
using SDVCommon.Tooltip;

namespace PlantingDay.ToolTip_Sections
{
    // Various planting / harvesting requirements e.g. trellis, multi-color sprites
    internal class PlantFeaturesSection
    {
        public static List<TooltipElement> Build(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            //Trellis
            if (plant.Data.Trellis && 
                plant.Data.PlantType == PlantType.Crop)
            {
                list.Add(new TooltipElement
                {
                    Icon = TooltipIcons.Get(IconKey.Trellis),
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.RequiresTrellis)
                    ),
                });
            }

            //No watering
            if (!plant.Data.NeedsWatering && 
                plant.Data.PlantType == PlantType.Crop)
            {
                list.Add(new TooltipElement
                {
                    Icon = TooltipIcons.Get(IconKey.Watercan),
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.NoWatering)
                    ),
                });
            }

            //Harvest with scythe
            if (plant.Data.NeedsScythe && 
                plant.Data.PlantType == PlantType.Crop)
            {
                list.Add(new TooltipElement
                {
                    Icon = TooltipIcons.Get(IconKey.Scythe),
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.HarvestWithScythe)
                    ),
                });
            }

            //Multicolor sprites
            if (plant.Data.MultiSprite > 0)
            {
                list.Add(new TooltipElement
                {
                    Icon = TooltipIcons.Get(IconKey.Rainbow),
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.MultiSprite),
                        plant.Data.MultiSprite
                    ),
                });
            }

            return list;
        }
    }
}
