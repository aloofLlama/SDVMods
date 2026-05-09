using HarvestHelper.Helpers;
using SDVCommon.GameData;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Models.Wrappers;

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
            string harvestId = harvest.Data.HarvestId;

            // TODOAdd about only displaying if seed cost is half harvest price

            //var seedInfo = PlantInfoBuilder.LookupFromKey(harvestId);

            //if (seedInfo == null)
            //    return list; // no harvest info found

            //int? minSeedPrice = EconomicsHelper.GetMinSeedPriceFromMainVendors(seedInfo);



            //var seedData = IdHelper.GetSeedDataForHarvest(harvestId);
            string? seedId = harvest.Data.SeedId;

            if (seedId == null)
                return list;

            var seedObject = GameObjectInfo.FromObject(seedId);

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
                Icon = TooltipIcons.GetIconForGameObject("(BC)25", 1.4f), //seedmaker
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.Owned),
                    owned)
            });

            return list;
        }


    }
}
