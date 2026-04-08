using PlantingDay.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.GameData.QuantityModifier;

namespace PlantingDay.Helpers
{
    public static class VendorHelper
    {
        //TODO - check crops from all mods. uncle iron sugarcane not working
        //TODO - check sunberry shops

        // Get seed purchase info from the game data and save to plant database
        public static List<PurchaseInfo> GetPurchaseInfo(string itemId)
        {
            var shops = Game1.content.Load<Dictionary<string, ShopData>>("Data/Shops");
            return BuildPurchaseInfo(itemId, shops);
        }

        public static List<PurchaseInfo> BuildPurchaseInfo(
            string itemId,
            Dictionary<string, ShopData> shops)

        {
            var results = new List<PurchaseInfo>();

            foreach (var (shopId, shop) in shops)
            {

                foreach (var entry in shop.Items)
                {
                    //Shop IDs have (O) prefixes, so need to adjust for comparison
                    bool directMatch = IdHelper.CanonicalItemId(entry.ItemId) == itemId;
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
                        //if (entry.Price >= 0)
                        //{
                        //    info.GoldPrice = entry.Price;
                        //    ModEntry.Instance.Monitor.Log($"Used entry.Price for:         {info.GoldPrice}g   {itemId}                   {info.VendorId}", LogLevel.Info);
                        //}
                        //else
                        //{
                            info.GoldPrice = GetDefaultShopPrice(itemId);
                            ModEntry.Instance.Monitor.Log($"Used GetDefaultShopPrice for: {info.GoldPrice}g   {itemId}                      {info.VendorId}", LogLevel.Info);
                        //}
                        }

                    // Apply price modifiers
                    if (entry.PriceModifiers != null)
                    {
                        int basePrice = info.GoldPrice ?? 0;
                        foreach (var mod in entry.PriceModifiers)
                        {

                            switch (mod.Modification)
                            {
                                case ModificationType.Add:
                                    info.GoldPrice += (int)mod.Amount;
                                    break;

                                case ModificationType.Subtract:
                                    info.GoldPrice -= (int)mod.Amount;
                                    break;

                                case ModificationType.Multiply:
                                    info.GoldPrice = (int)(basePrice * mod.Amount);
                                    break;

                                case ModificationType.Divide:
                                    info.GoldPrice = (int)(basePrice / mod.Amount);
                                    break;
                            }
                        }
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

                // Collapse Night Market (all boats)
                _ when shopId.Contains("NightMarket", StringComparison.OrdinalIgnoreCase)
                    => "Night Market",


                //sunberry
                "skellady.SBVCP_AriMarket" => "Ari",
                "skellady.SBVCP_JumanaShop" => "Jumana",

                _ => shopId // fallback for modded shops
            };
        }

        public static int GetDefaultShopPrice(string itemId)
        {
            string gameId = IdHelper.ToGameId(itemId);
            var item = ItemRegistry.Create(gameId);

            if (item == null)
                return 0;

            // Fake shop data (Pierre)
            var shopData = new ShopData();

            // Fake shop item entry
            var itemData = new ShopItemData
            {
                Id = gameId,
                ItemId = gameId,
                Price = -1,
                IgnoreShopPriceModifiers = false,
                UseObjectDataPrice = false
            };

            // ItemQueryResult requires the item
            var output = new ItemQueryResult(item);

            // Compute the real base price
            int price = ShopBuilder.GetBasePrice(
                output,
                shopData,
                itemData,
                item,
                outOfSeasonPrice: false,
                useObjectDataPrice: false
            );

            return price;
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

        public static string VendorKey(PurchaseInfo v)
        {
            if (IsPierre(v))
                return "SeedShop";

            if (IsNightMarket(v))
                return "NightMarket";

            if (IsDesertFestival(v))
                return "DesertFestival";

            return v.VendorId;
        }

        public static int SortKey(PurchaseInfo v)
        {
            // 0 — Pierre always first
            if (Helpers.VendorHelper.IsPierre(v))
                return 0;

            //// 4 — Night Market always last
            //if (Helpers.VendorHelper.IsNightMarket(v))
            //    return 4;

            // 1 — Gold vendors (Joja, Traveling Cart, etc.)
            if (v.GoldPrice.HasValue)
                return 1;

            // 3 — Trade vendors (Desert Trader, Island Trader, Qi trade shops)
            if (v.TradeAmount > 0)
                return 3;

            // 2 — Everything else
            return 2;
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
