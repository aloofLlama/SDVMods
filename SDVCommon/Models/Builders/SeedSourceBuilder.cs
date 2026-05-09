using SDVCommon.Models.Tooltip;
using SDVCommon.Models.Wrappers;
using SDVCommon.GameData;
using SDVData;

namespace SDVCommon.Models.Builders
{
    public static class SeedSourceBuilder
    {

        public static List<object> Build(PlantInfo plant)
        {
            // 1. Filter → collapse → sort vendors
            var vendors = plant.PurchaseOptions
                .GroupBy(v => Vendor.VendorKey(v))
                .Select(g => g.First())
                .OrderBy(v => CurrencySortKey(v))
                .ThenBy(v => VendorSortKey(v))
                .ThenBy(v => v.Data.VendorName)
                .Cast<object>()
                .ToList();

            // 2. Add monsters
            var monsters = plant.MonsterDrops
                .OrderByDescending(m => m.Data.DropChance)
                .Cast<object>()
                .ToList();

            // 3. Combine and return
            return vendors
                .Concat(monsters)
                .OrderBy(s => FinalSortKey(s))
                .ToList();
        }


        private static int CurrencySortKey(PurchaseInfo v)
        {
            var d = v.Data;

            if (d.GoldPrice.HasValue)
                return 0;   // gold vendors first

            if (d.TradeAmount > 0)
                return 1;   // trade vendors second

            return 2;       // everything else (shouldn’t happen)
        }

        //──────────────────────────────────────────────
        // Vendor sort key (before monsters)
        //──────────────────────────────────────────────
        private static int VendorSortKey(PurchaseInfo v)
        {
            return v.Data.Type switch
            {
                PurchaseInfoData.VendorType.Pierre => 0,
                PurchaseInfoData.VendorType.Joja => 1,
                PurchaseInfoData.VendorType.ValleyFair => 2,
                PurchaseInfoData.VendorType.Other => 3,
                PurchaseInfoData.VendorType.DesertFestival => 4,

                PurchaseInfoData.VendorType.TravelingCart => 6, //monsters set before this, below
                PurchaseInfoData.VendorType.NightMarket => 7,

                _ => 0
            };
        }

        //──────────────────────────────────────────────
        // Final sort key (vendors → monsters → TC → NM)
        //──────────────────────────────────────────────
        private static int FinalSortKey(object source)
        {
            return source switch
            {
                PurchaseInfo v => VendorSortKey(v), // vendors first
                MonsterDropInfo => 5,               // monsters after vendors, before icons
                _ => 999
            };
        }
    }
}