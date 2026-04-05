using PlantingDay.Models;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Helpers
{
    public static class VendorHelper
    {
        //TODO - check crops from all mods. uncle iron sugarcane not working
        //TODO - check sunberry shops
        //TODO FIX BOTH PIERRE AND OTHER SHOP PRICE

        // Get seed purchase info from the game data and save to plant database
        public static List<PurchaseInfo> GetPurchaseInfo(string itemId)
        {
            var results = new List<PurchaseInfo>();
            var shops = Game1.content.Load<Dictionary<string, ShopData>>("Data/Shops");

            foreach (var (shopId, shop) in shops)
            {

                foreach (var entry in shop.Items)
                {
                    bool directMatch = IdHelper.NormalizeItemId(entry.ItemId) == IdHelper.NormalizeItemId(itemId);
                    bool wildcardMatch = entry.ItemId == "ALL_ITEMS (O)" && ItemMatchesWildcard(itemId, entry);

                    if (!directMatch && !wildcardMatch)
                        continue;

                    var info = new PurchaseInfo
                    {
                        VendorId = shopId,
                        VendorName = GetVendorName(shopId),
                        Condition = entry.Condition
                    };

                    // Trade (non-gold)
                    if (!string.IsNullOrEmpty(entry.TradeItemId))
                    {
                        info.TradeItemId = entry.TradeItemId;
                        info.TradeAmount = entry.TradeItemAmount;
                    }

                    // Gold 
                    else
                    {
                        if (entry.Price >= 0)
                            info.GoldPrice = entry.Price;
                        else
                            info.GoldPrice = GetDefaultShopPrice(shopId, itemId);
                    }

                    results.Add(info);
                }

            }


            return results;
        }
        public static string GetVendorName(string shopId)
        {
            return shopId switch
            {
                "SeedShop" => "Pierre",
                "Joja" => "Joja",
                "Sandy" => "Oasis",
                "AnimalShop" => "Marnie",
                "IslandTrade" => "Island Trader",
                "DesertTrade" => "Desert Trader",
                "Traveler" => "Traveling Cart",

                //festivals
                "Festival_Luau_Pierre" => "Luau",
                "Festival_EggFestival_Pierre" => "Egg Festival",
                "Festival_StardewValleyFair_StarTokens" => "Valley Fair",

                // Collapse Desert Festival
                _ when shopId.StartsWith("DesertFestival", StringComparison.OrdinalIgnoreCase)
                    => string.Join(", ",
                    shopId
                    .Split(',')
                    .Select(id => id.Replace("DesertFestival_", ""))
            ),


                //sunberry
                "skellady.SBVCP_AriMarket" => "Ari",
                "skellady.SBVCP_JumanaShop" => "Jumana",

                _ => shopId // fallback for modded shops
            };
        }

        public static int GetDefaultShopPrice(string shopId, string itemId)
        {
            var obj = ItemRegistry.Create(itemId) as StardewValley.Object;
            if (obj == null)
                return 0;

            int sellPrice = obj.sellToStorePrice();
            //ModEntry.Instance.Monitor.Log($"sell price: {sellPrice}", LogLevel.Info);

            return shopId switch
            {
                "SeedShop" => sellPrice * 2,
                "Joja" => sellPrice * 3,
                "Sandy" => sellPrice * 2,
                //"Dwarf" => sellPrice * 2,
                _ => sellPrice * 2
            };
        }

        private static bool ItemMatchesWildcard(string itemId, ShopItemData entry)
        {
            // Wildcard entries must have PerItemCondition
            if (string.IsNullOrWhiteSpace(entry.PerItemCondition))
                return false;

            // Create the item instance
            Item item = ItemRegistry.Create(itemId);
            if (item == null)
                return false;

            // Get the item's context tags
            var tags = item.GetContextTags();

            // Split the PerItemCondition into individual rules
            // Example:
            // "ITEM_CONTEXT_TAG Target cornucopia_shop_pierre, ITEM_CONTEXT_TAG Target cornucopia_season_spring, !ITEM_CONTEXT_TAG Target cornucopia_shop_pierre_generic_banned"
            var rules = entry.PerItemCondition
                .Split(',')
                .Select(r => r.Trim())
                .Where(r => r.Length > 0);

            foreach (var rule in rules)
            {
                bool negated = rule.StartsWith("!");
                string cleanRule = negated ? rule.Substring(1).Trim() : rule;

                if (!cleanRule.StartsWith("ITEM_CONTEXT_TAG", StringComparison.OrdinalIgnoreCase))
                    continue;

                int idx = cleanRule.IndexOf("Target ", StringComparison.OrdinalIgnoreCase);
                if (idx < 0)
                    continue;

                string requiredTag = cleanRule.Substring(idx + "Target ".Length).Trim();

                bool hasTag = tags.Contains(requiredTag);

                // If rule is negated, item must NOT have the tag
                if (negated)
                {
                    if (hasTag)
                        return false;
                }
                else
                {
                    if (!hasTag)
                        return false;
                }
            }

            // Passed all rules
            return true;
        }

        public static bool IsPierre(PurchaseInfo v) =>
            v.VendorId == "SeedShop";
        public static bool IsNightMarket(PurchaseInfo v) =>
            v.VendorId.Contains("NightMarket", StringComparison.OrdinalIgnoreCase);
        public static bool IsValleyFair(PurchaseInfo v) =>
            v.VendorId.Contains("StardewValleyFair_StarTokens", StringComparison.OrdinalIgnoreCase);
        public static bool IsDesertFestival(PurchaseInfo v) =>
            v.VendorId.Contains("DesertFestival", StringComparison.OrdinalIgnoreCase);

    }
}
