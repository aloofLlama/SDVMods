using System;
using System.Linq;
using PlantingDay.Models.Runtime;

namespace PlantingDay.Helpers.SeedSource
{
    public static class VendorHelper
    {
        // ------------------------------------------------------------
        // VENDOR NAME RESOLUTION (UI metadata)
        // ------------------------------------------------------------
        public static string GetVendorName(string shopId)
        {
            return shopId switch
            {
                "SeedShop" => "Pierre",
                "Sandy" => "Oasis",
                "AnimalShop" => "Marnie",
                "IslandTrade" => "Island Trader",
                "DesertTrade" => "Desert Trader",
                "Traveler" => "Traveling Cart",

                // Festivals
                "Festival_Luau_Pierre" => "Luau",
                "Festival_EggFestival_Pierre" => "Egg Festival",
                "Festival_StardewValleyFair_StarTokens" => "Valley Fair",
                "Festival_FlowerDance_Pierre" => "Flower Dance",

                // Collapse Desert Festival
                _ when shopId.StartsWith("DesertFestival", StringComparison.OrdinalIgnoreCase)
                    => string.Join(", ",
                        shopId.Split(',')
                              .Select(id => id.Replace("DesertFestival_", ""))
                    ),

                // Collapse Night Market (all boats)
                _ when shopId.Contains("NightMarket", StringComparison.OrdinalIgnoreCase)
                    => "Night Market",

                // Sunberry
                "skellady.SBVCP_AriMarket" => "Ari",
                "skellady.SBVCP_JumanaShop" => "Jumana",

                _ => shopId // fallback for modded shops
            };
        }

        // ------------------------------------------------------------
        // GROUPING KEY (used to collapse duplicates)
        // ------------------------------------------------------------
        public static string VendorKey(PurchaseInfoRuntime info)
        {
            var id = info.Data.VendorId;

            if (IsPierre(id))
                return "SeedShop";

            if (IsNightMarket(id))
                return "NightMarket";

            if (IsDesertFestival(id))
                return "DesertFestival";

            return id;
        }

        // ------------------------------------------------------------
        // SORTING KEY (used by VendorListBuilder)
        // ------------------------------------------------------------
        public static int SortKey(PurchaseInfoRuntime info)
        {
            var data = info.Data;

            // 0 — Pierre always first
            if (IsPierre(data.VendorId))
                return 0;

            // 1 — Gold vendors
            if (data.GoldPrice.HasValue)
                return 1;

            // 3 — Trade vendors
            if (data.TradeAmount > 0)
                return 3;

            // 2 — Everything else
            return 2;
        }

        // ------------------------------------------------------------
        // CLASSIFICATION HELPERS
        // ------------------------------------------------------------
        public static bool IsPierre(string vendorId) =>
            vendorId == "SeedShop";

        public static bool IsNightMarket(string vendorId) =>
            vendorId.Contains("NightMarket", StringComparison.OrdinalIgnoreCase);

        public static bool IsValleyFair(string vendorId) =>
            vendorId.Contains("StardewValleyFair_StarTokens", StringComparison.OrdinalIgnoreCase);

        public static bool IsDesertFestival(string vendorId) =>
            vendorId.Contains("DesertFestival", StringComparison.OrdinalIgnoreCase);
    }
}
