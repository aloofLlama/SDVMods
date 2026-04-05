using PlantingDay.Helpers;
using PlantingDay.Models;
using StardewValley.GameData.Crops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.ToolTip_Sections
{
    // Various planting / harvesting requirements e.g. trellis, multi-color sprites
    internal class PlantFeaturesSection
    {
        public static List<TooltipElement> Build(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            //Trellis
            if (plant.Trellis && 
                plant.PlantType == PlantType.Crop)
            {
                list.Add(new TooltipElement
                {
                    IconRef = TooltipIcons.Trellis,
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.RequiresTrellis)
                    ),
                });
            }

            //No watering
            if (!plant.NeedsWatering && 
                plant.PlantType == PlantType.Crop)
            {
                list.Add(new TooltipElement
                {
                    IconRef = TooltipIcons.Watercan,
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.NoWatering)
                    ),
                });
            }

            //Harvest with scythe
            if (plant.Scythe == HarvestMethod.Scythe && 
                plant.PlantType == PlantType.Crop)
            {
                list.Add(new TooltipElement
                {
                    IconRef = TooltipIcons.Scythe,
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.HarvestWithScythe)
                    ),
                });
            }

            //Multicolor sprites
            if (plant.MultiSprite > 0)
            {
                list.Add(new TooltipElement
                {
                    IconRef = TooltipIcons.Rainbow,
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.MultiSprite),
                        plant.MultiSprite
                    ),
                });
            }

            return list;
        }
    }
}
