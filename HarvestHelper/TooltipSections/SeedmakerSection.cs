using HarvestHelper.Helpers;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVData;

using SDVCommon.Helpers.Tooltip;
using SDVCommon.Models.Tooltip;

namespace HarvestHelper.TooltipSections
{
    public static class SeedmakerSection
    {
        //Only want this if a) seedmaker is allowed, and b) seed cost is at least half harvest price
        public static List<TooltipElement> Build(HarvestInfo harvest, StardewValley.Object obj)
        {
            var list = new List<TooltipElement>();
            string harvestQId = harvest.HarvestQId;

            // TODOAdd about only displaying if seed cost is half harvest price

            //var seedInfo = PlantInfoBuilder.LookupFromKey(harvestId);

            //if (seedInfo == null)
            //    return list; // no harvest info found

            //int? minSeedPrice = EconomicsHelper.GetMinSeedPriceFromMainVendors(seedInfo);



            //var seedData = IdHelper.GetSeedDataForHarvest(harvestId);
            string? seedQId = harvest.SeedQId;

            if (seedQId == null)
                return list;

            var seedObject = GameObject.GetObjectInfo(seedQId);

            //don't show if seedmaker banned
            if (seedObject?.ContextTags?.Contains("seedmaker_banned") == true)
                return list;

            int realHarvestPrice = obj.sellToStorePrice();
            //int seedPrice = seedData.SeedPrice;

            int owned = 0;


            if (seedQId != null)
            {
                owned = Inventory.CountOwned(seedQId);
            }

            list.Add(new TooltipElement
            {
                Icon = IconRegistry.GetIcon("(BC)25")?.WithScale(1.4f), // seedmaker icon
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.Owned),
                    owned)
            });

            return list;
        }


    }
}
