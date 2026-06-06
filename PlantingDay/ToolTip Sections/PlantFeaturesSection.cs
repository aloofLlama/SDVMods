using PlantingDay.Helpers;
using SDVCommon.Icons;
using SDVData;
using SDVCommon.Models.Tooltip;
using SDVCommon.Models.Wrappers;

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
                    Icon = IconKey.Trellis.GetIcon(),
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
                    Icon = IconKey.Watercan.GetIcon(),
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
                    Icon = IconKey.Scythe.GetIcon(),
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
                    Icon = IconKey.Rainbow.GetIcon(),
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
