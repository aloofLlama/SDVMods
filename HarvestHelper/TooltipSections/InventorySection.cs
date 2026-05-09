using HarvestHelper.Helpers;
using SDVCommon;
using SDVCommon.Helpers;
using SDVCommon.Models.Wrappers;
using SDVCommon.Models.Tooltip;


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

