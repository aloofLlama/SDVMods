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
            string harvestId = harvest.HarvestId;

            // TODOAdd about only displaying if seed cost is half harvest price

            //var seedInfo = PlantInfoBuilder.LookupFromKey(harvestId);

            //if (seedInfo == null)
            //    return list; // no harvest info found

            //int? minSeedPrice = EconomicsHelper.GetMinSeedPriceFromMainVendors(seedInfo);



            //var seedData = IdHelper.GetSeedDataForHarvest(harvestId);
            string? seedId = harvest.SeedId;

            if (seedId == null)
                return list;

            var seedObject = GameObject.FromObject(seedId);

            //don't show if seedmaker banned
            if (seedObject?.ContextTags?.Contains("seedmaker_banned") == true)
                return list;

            int realHarvestPrice = obj.sellToStorePrice();
            //int seedPrice = seedData.SeedPrice;

            int owned = 0;


            if (seedId != null)
            {
                owned = Inventory.CountOwned(seedId);
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
