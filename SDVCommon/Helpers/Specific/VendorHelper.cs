using SDVCommon.Models.Wrappers;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Shops;
using static SDVData.PurchaseInfoData;

namespace SDVCommon.Helpers.Specific
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
                //"Traveler" => "Traveling Cart",

                // Festivals
                "Festival_Luau_Pierre" => "Luau",
                "Festival_EggFestival_Pierre" => "Egg Festival",
                "Festival_StardewValleyFair_StarTokens" => "Valley Fair",
                "Festival_FlowerDance_Pierre" => "Flower Dance",
                "Festival_FestivalOfIce_TravelingMerchant" => "Festival of Ice",


                // Remove Desert Festival header
                _ when shopId.StartsWith("DesertFestival", StringComparison.OrdinalIgnoreCase)
                    => string.Join(", ",
                        shopId.Split(',')
                              .Select(id => id.Replace("DesertFestival_", ""))
                    ),

                // Expanded
                "FlashShifter.StardewValleyExpandedCP_YellowJunimoVendor" => "Junimo Woods",

                // Sunberry
                "skellady.SBVCP_AriMarket" => "Ari",
                "skellady.SBVCP_JumanaShop" => "Jumana",

                _ => shopId // fallback
            };
        }

        // ------------------------------------------------------------
        // GROUPING KEY (used to collapse duplicates)
        // ------------------------------------------------------------
        public static string VendorKey(PurchaseInfo purchase)
        {
            return GetVendorType(purchase.Data.VendorId) switch
            {
                VendorType.Pierre => "SeedShop",
                VendorType.NightMarket => "NightMarket",
                VendorType.DesertFestival => "DesertFestival",
                _ => purchase.Data.VendorId
            };
        }

        // ------------------------------------------------------------
        // SORTING KEY (used by VendorListBuilder)
        // ------------------------------------------------------------
        //public static int SortKey(PurchaseInfoRuntime v)
        //{
        //    return GetVendorType(v.Data.VendorId) switch
        //    {
        //        VendorType.Pierre => 0,
        //        //VendorType.NightMarket => 4, // always last
        //        //VendorType.TravelingCart => 1, // gold vendor
        //        VendorType.ValleyFair => 3, // trade vendor
        //        VendorType.DesertFestival => 3,
        //        _ => v.Data.GoldPrice.HasValue ? 1 : 2
        //    };
        //}

        // ------------------------------------------------------------
        // CLASSIFICATION HELPERS
        // ------------------------------------------------------------
        public static VendorType GetVendorType(string vendorId)
        {
            if (vendorId == "SeedShop")
                return VendorType.Pierre;

            if (vendorId.Contains ("Joja", StringComparison.OrdinalIgnoreCase))
                return VendorType.Joja;

            if (vendorId.Contains("FlashShifter.StardewValleyExpandedCP_ZoeyVendor", StringComparison.OrdinalIgnoreCase))
                return VendorType.JojaEmporium;

            if (vendorId.Contains("NightMarket", StringComparison.OrdinalIgnoreCase))
                return VendorType.NightMarket;

            if (vendorId.Contains("Traveler", StringComparison.OrdinalIgnoreCase))
                return VendorType.TravelingCart;

            if (vendorId.Contains("DesertFestival", StringComparison.OrdinalIgnoreCase))
                return VendorType.DesertFestival;

            if (vendorId.Contains("StardewValleyFair_StarTokens", StringComparison.OrdinalIgnoreCase))
                return VendorType.ValleyFair;

            return VendorType.Other;
        }


        // ------------------------------------------------------------
        // Wildcard matching
        // ------------------------------------------------------------
        public static bool ItemMatchesWildcard(string itemId, ShopItemData entry)
        {
            if (string.IsNullOrWhiteSpace(entry.PerItemCondition))
                return false;

            if (!entry.PerItemCondition.Contains("ITEM_CONTEXT_TAG", StringComparison.OrdinalIgnoreCase))
                return false;

            Item item = ItemRegistry.Create(IdHelper.ToGameId(itemId));
            if (item == null)
                return false;

            var tags = item.GetContextTags();

            var rules = entry.PerItemCondition
                .Split(',')
                .Select(r => r.Trim())
                .Where(r => r.Length > 0);

            foreach (var rule in rules)
            {
                bool negated = rule.StartsWith("!");
                string cleanRule = negated ? rule[1..].Trim() : rule;

                if (!cleanRule.StartsWith("ITEM_CONTEXT_TAG", StringComparison.OrdinalIgnoreCase))
                    continue;

                int idx = cleanRule.IndexOf("Target ", StringComparison.OrdinalIgnoreCase);
                if (idx < 0)
                    continue;

                string requiredTag = cleanRule[(idx + "Target ".Length)..].Trim();
                bool hasTag = tags.Contains(requiredTag);

                if (negated && hasTag)
                    return false;

                if (!negated && !hasTag)
                    return false;
            }

            return true;
        }

        // ------------------------------------------------------------
        // Condition matching
        // ------------------------------------------------------------
        public static bool EvaluatePerItemCondition(string? condition, string itemId)
        {
            if (string.IsNullOrEmpty(condition))
                return true;

            if (condition.StartsWith("ANY"))
            {
                var parts = condition.Split('"', StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in parts)
                {
                    if (part.Contains("ITEM_ID Target"))
                    {
                        var target = part.Split(' ', StringSplitOptions.RemoveEmptyEntries).Last();
                        if (IdHelper.CanonicalItemId(itemId)
                                .Equals(IdHelper.CanonicalItemId(target), StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }

                return false;
            }

            return false;
        }

        // ------------------------------------------------------------
        // Random Pool matching (Traveling Cart stock pool)
        // ------------------------------------------------------------
        public static bool MatchesRandomPool(string itemId, string? entryItemId)
        {
            if (string.IsNullOrEmpty(entryItemId))
                return false;

            if (!entryItemId.Contains("{{Random:", StringComparison.OrdinalIgnoreCase))
                return false;

            int start = entryItemId.IndexOf(':');
            int end = entryItemId.IndexOf("}}");

            if (start < 0 || end < 0)
                return false;

            string inner = entryItemId.Substring(start + 1, end - start - 1);
            string[] parts = inner.Split(',');

            foreach (var p in parts)
            {
                string candidate = p.Trim();
                if (IdHelper.CanonicalItemId(candidate)
                        .Equals(IdHelper.CanonicalItemId(itemId), StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
        public static int GetMinYear(PlantInfo plant)
        {
            int minYear = int.MaxValue;

            foreach (var opt in plant.PurchaseOptions)
            {
                var cond = opt.Data.Condition;
                if (string.IsNullOrWhiteSpace(cond))
                    continue;

                var tokens = cond.Split(',', ' ', '\t');

                for (int i = 0; i < tokens.Length - 1; i++)
                {
                    if (tokens[i] == "YEAR" && int.TryParse(tokens[i + 1], out int y))
                    {
                        minYear = Math.Min(minYear, y);
                    }
                }
            }

            return minYear == int.MaxValue ? 1 : minYear;
        }


    }
}
