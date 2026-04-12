using SDVData;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using SDVCommon.Helpers;
using PlantingDay.Helpers;

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
                    bool wildcardMatch =
                        entry.ItemId == "ALL_ITEMS (O)" &&
                        (
                            ItemMatchesWildcard(itemId, entry) ||   // context-tag based
                            EvaluatePerItemCondition(entry.PerItemCondition, itemId) // ITEM_ID based
                        );

                    if (!directMatch && !wildcardMatch)
                        continue;

                    // Build POCO
                    var info = new PurchaseInfoData
                    {
                        VendorId = shopId,
                        VendorName = VendorHelper.GetVendorName(shopId),
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
                        
                        var item = ItemRegistry.Create(IdHelper.ToGameId(itemId));
                        if (item != null)
                        {
                            var output = new ItemQueryResult(item);

                            // First: calculate the real game price
                            int calculatedPrice = ShopBuilder.GetBasePrice(
                                output,
                                shop,
                                entry,
                                item,
                                outOfSeasonPrice: false,
                                useObjectDataPrice: entry.UseObjectDataPrice
                            );

                            // Second: check for override
                            if (SeedPriceOverrides.Overrides.TryGetValue((itemId, shopId), out int overridePrice))
                            {
                                info.GoldPrice = overridePrice;
                            }
                            else
                            {
                                info.GoldPrice = calculatedPrice;
                            }
                        }
                    
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
        // WILDCARD MATCHING
        // ------------------------------------------------------------
        private static bool ItemMatchesWildcard(string itemId, ShopItemData entry)
        {
            if (string.IsNullOrWhiteSpace(entry.PerItemCondition))
                return false;

            // Only handle ITEM_CONTEXT_TAG rules here.
            // If the condition has no context-tag rules, this wildcard matcher should not apply.
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
        // CONDITION MATCHING
        // ------------------------------------------------------------
        private static bool EvaluatePerItemCondition(string? condition, string itemId)
        {
                if (string.IsNullOrEmpty(condition))
                    return true;

                // Example:
                // ANY "ITEM_ID Target Cornucopia_Pansy" "ITEM_ID Target Cornucopia_Violet"
                if (condition.StartsWith("ANY"))
                {
                    // Extract everything inside quotes
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

        }
    }
