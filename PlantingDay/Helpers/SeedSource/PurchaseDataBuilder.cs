using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using PlantingDay.Models;

namespace PlantingDay.Helpers.SeedSource
{
    internal static class PurchaseDataBuilder
    {
        // ------------------------------------------------------------
        // PUBLIC ENTRY POINT
        // ------------------------------------------------------------
        public static List<PurchaseInfoData> GetPurchaseInfo(string itemId)
        {
            var shops = Game1.content.Load<Dictionary<string, ShopData>>("Data/Shops");
            return BuildPurchaseInfo(itemId, shops);
        }

        // ------------------------------------------------------------
        // CORE EXTRACTION LOGIC
        // ------------------------------------------------------------
        public static List<PurchaseInfoData> BuildPurchaseInfo(
            string itemId,
            Dictionary<string, ShopData> shops)
        {
            var results = new List<PurchaseInfoData>();

            foreach (var (shopId, shop) in shops)
            {
                foreach (var entry in shop.Items)
                {
                    // Match direct item or wildcard
                    bool directMatch = IdHelper.CanonicalItemId(entry.ItemId) == itemId;
                    bool wildcardMatch = entry.ItemId == "ALL_ITEMS (O)" &&
                                         ItemMatchesWildcard(itemId, entry);

                    if (!directMatch && !wildcardMatch)
                        continue;

                    // Build POCO
                    var info = new PurchaseInfoData
                    {
                        VendorId = shopId,
                        Condition = entry.Condition
                    };

                    // Trade (non-gold)
                    if (!string.IsNullOrEmpty(entry.TradeItemId))
                    {
                        info.TradeItemId = entry.TradeItemId;
                        info.TradeAmount = entry.TradeItemAmount;
                    }
                    else
                    {
                        // Gold price (default or explicit)
                        info.GoldPrice = entry.Price >= 0
                            ? entry.Price
                            : GetDefaultShopPrice(itemId);
                    }

                    // Apply price modifiers
                    if (entry.PriceModifiers != null)
                    {
                        int basePrice = info.GoldPrice ?? 0;

                        foreach (var mod in entry.PriceModifiers)
                        {
                            switch (mod.Modification)
                            {
                                case StardewValley.GameData.QuantityModifier.ModificationType.Add:
                                    info.GoldPrice += (int)mod.Amount;
                                    break;

                                case StardewValley.GameData.QuantityModifier.ModificationType.Subtract:
                                    info.GoldPrice -= (int)mod.Amount;
                                    break;

                                case StardewValley.GameData.QuantityModifier.ModificationType.Multiply:
                                    info.GoldPrice = (int)(basePrice * mod.Amount);
                                    break;

                                case StardewValley.GameData.QuantityModifier.ModificationType.Divide:
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

        // ------------------------------------------------------------
        // DEFAULT PRICE CALCULATION (FAKE SHOP)
        // ------------------------------------------------------------
        public static int GetDefaultShopPrice(string itemId)
        {
            string gameId = IdHelper.ToGameId(itemId);
            var item = ItemRegistry.Create(gameId);

            if (item == null)
                return 0;

            var shopData = new ShopData();
            var itemData = new ShopItemData
            {
                Id = gameId,
                ItemId = gameId,
                Price = -1,
                IgnoreShopPriceModifiers = false,
                UseObjectDataPrice = false
            };

            var output = new ItemQueryResult(item);

            return ShopBuilder.GetBasePrice(
                output,
                shopData,
                itemData,
                item,
                outOfSeasonPrice: false,
                useObjectDataPrice: false
            );
        }

        // ------------------------------------------------------------
        // WILDCARD MATCHING
        // ------------------------------------------------------------
        private static bool ItemMatchesWildcard(string itemId, ShopItemData entry)
        {
            if (string.IsNullOrWhiteSpace(entry.PerItemCondition))
                return false;

            Item item = ItemRegistry.Create(itemId);
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
                string cleanRule = negated ? rule.Substring(1).Trim() : rule;

                if (!cleanRule.StartsWith("ITEM_CONTEXT_TAG", StringComparison.OrdinalIgnoreCase))
                    continue;

                int idx = cleanRule.IndexOf("Target ", StringComparison.OrdinalIgnoreCase);
                if (idx < 0)
                    continue;

                string requiredTag = cleanRule.Substring(idx + "Target ".Length).Trim();
                bool hasTag = tags.Contains(requiredTag);

                if (negated && hasTag)
                    return false;

                if (!negated && !hasTag)
                    return false;
            }

            return true;
        }
    }
}
