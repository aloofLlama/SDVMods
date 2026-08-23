using HarvestHelper.Helpers;
using SDVCommon.Helpers;
using SDVCommon.GameData;
using SDVData;
using SDVCommon.Models.Tooltip;


namespace HarvestHelper.TooltipSections
{
    public static class InventorySection
    {
        public static List<TooltipElement> Build(HarvestInfo harvest, StardewValley.Object obj)
        {
            var list = new List<TooltipElement>();
            string harvestQId = harvest.HarvestQId;
            int owned = Inventory.CountOwned(harvestQId);

            list.Add(new TooltipElement
            {
                Icon = IconRegistry.GetIcon(harvestQId),
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.Owned),
                    owned)
            });

            return list;
        }
    }
}

