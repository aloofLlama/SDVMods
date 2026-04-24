using PlantingDay.Helpers;
using SDVCommon.Helpers;
using SDVData;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static SDVData.PurchaseInfoData;

namespace PlantingDay.Helpers.SeedSource
{
    internal static class PurchaseDataBuilder
    {
        public static List<PurchaseInfoData> GetPurchaseInfo(string itemId)
        {
            var shops = Game1.content.Load<Dictionary<string, ShopData>>("Data/Shops");
            return BuildPurchaseInfo(itemId, shops);
        }

        public static List<PurchaseInfoData> BuildPurchaseInfo(
            string itemId,
            Dictionary<string, ShopData> shops)
        {
            var results = new List<PurchaseInfoData>();

            foreach (var (shopId, shop) in shops)
            {
                foreach (var entry in shop.Items)
                {
                    // -----------------------------
                    // Is the shop entry a seed
                    // -----------------------------
                    bool directMatch =
                        IdHelper.CanonicalItemId(entry.ItemId)
                            .Equals(IdHelper.CanonicalItemId(itemId), StringComparison.OrdinalIgnoreCase);

                    bool wildcardMatch =
                        entry.ItemId == "ALL_ITEMS (O)" &&
                        (
                            VendorHelper.ItemMatchesWildcard(itemId, entry) ||
                            VendorHelper.EvaluatePerItemCondition(entry.PerItemCondition, itemId)
                        );

                    bool randomPoolMatch = false;
                    if (!directMatch && !wildcardMatch)
                        randomPoolMatch = VendorHelper.MatchesRandomPool(itemId, entry.ItemId);

                    if (!directMatch && !wildcardMatch && !randomPoolMatch)
                        continue;

                    // -----------------------------
                    // CREATE PURCHASE INFO ENTRY
                    // -----------------------------
                    var info = new PurchaseInfoData
                    {
                        VendorId = shopId,
                        VendorName = VendorHelper.GetVendorName(shopId),
                        Condition = entry.Condition,

                        //IsNightMarket = VendorHelper.IsNightMarket(shopId),
                        //IsTravelingCartSpecial = false // set below
                    };
                    info.Type = VendorHelper.GetVendorType(shopId);


                    // -----------------------------
                    // PRICE OR TRADE
                    // -----------------------------
                    if (!string.IsNullOrEmpty(entry.TradeItemId))
                    {
                        info.TradeItemId = entry.TradeItemId;
                        info.TradeAmount = entry.TradeItemAmount;
                    }
                    else
                    {
                        info.GoldPrice = GetGoldPrice(itemId, shop, entry);
                    }

                    results.Add(info);
                }
            }

            return results;
        }

        // --------------------------------------
        // PRICE CALCULATION
        // --------------------------------------
        private static int? GetGoldPrice(string itemId, ShopData shop, ShopItemData entry)
        {
            var item = ItemRegistry.Create(IdHelper.ToGameId(itemId));
            if (item == null)
                return null;

            var output = new ItemQueryResult(item);

            float value = ShopBuilder.GetBasePrice(
                output,
                shop,
                entry,
                item,
                outOfSeasonPrice: false,
                useObjectDataPrice: entry.UseObjectDataPrice
            );

            // shop-level modifiers
            if (!entry.IgnoreShopPriceModifiers && shop.PriceModifiers != null)
            {
                value = Utility.ApplyQuantityModifiers(
                    value,
                    shop.PriceModifiers,
                    shop.PriceModifierMode,
                    null, null,
                    item,
                    null,
                    Utility.CreateDaySaveRandom()
                );
            }

            // item-level modifiers
            if (entry.PriceModifiers != null)
            {
                value = Utility.ApplyQuantityModifiers(
                    value,
                    entry.PriceModifiers,
                    entry.PriceModifierMode,
                    null, null,
                    item,
                    null,
                    Utility.CreateDaySaveRandom()
                );
            }

            return (int)value;
        }

        // --------------------------------------
        // TRAVELING CART SPECIAL FLAG
        // --------------------------------------
        private static bool SetTravelingCartSpecialFlag(
            string itemId,
            string shopId,
            ShopItemData entry,
            bool directMatch,
            bool wildcardMatch,
            bool randomPoolMatch)
        {
            if (VendorHelper.GetVendorType(shopId) != VendorType.TravelingCart)
                return false;

            bool isSpecial = false;

            if (directMatch)
            {
                isSpecial = true;
                ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} matched direct entry.", LogLevel.Info);
            }

            if (wildcardMatch)
            {
                isSpecial = true;
                ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} matched wildcard entry.", LogLevel.Info);
            }

            if (randomPoolMatch)
            {
                isSpecial = true;
                ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} appears in Random ItemId pool: {entry.ItemId}", LogLevel.Info);
            }

            if (entry.PriceModifiers?.Count > 0)
            {
                isSpecial = true;
                ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} has price modifiers.", LogLevel.Info);
            }

            if (entry.PriceModifiers?.Any(m => m.RandomAmount?.Count > 0) == true)
            {
                isSpecial = true;
                ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} has RandomAmount pricing.", LogLevel.Info);
            }

            if (!string.IsNullOrEmpty(entry.PerItemCondition))
            {
                isSpecial = true;
                ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} has per-item conditions.", LogLevel.Info);
            }

            if (!isSpecial)
            {
                ModEntry.Instance.Monitor.Log($"[TC-NORMAL] {itemId} has a TC entry but no special rules.", LogLevel.Info);
            }

            return isSpecial;
        }
    }
}

//using PlantingDay.Helpers;
//using SDVCommon.Helpers;
//using SDVData;
//using StardewModdingAPI;
//using StardewValley;
//using StardewValley.GameData.Shops;
//using StardewValley.Internal;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace PlantingDay.Helpers.SeedSource
//{
//    internal static class PurchaseDataBuilder
//    {
//        public static List<PurchaseInfoData> GetPurchaseInfo(string itemId)
//        {
//            var shops = Game1.content.Load<Dictionary<string, ShopData>>("Data/Shops");
//            return BuildPurchaseInfo(itemId, shops);
//        }

//        public static List<PurchaseInfoData> BuildPurchaseInfo(
//            string itemId,
//            Dictionary<string, ShopData> shops)
//        {
//            var results = new List<PurchaseInfoData>();

//            foreach (var (shopId, shop) in shops)
//            {
//                foreach (var entry in shop.Items)
//                {
//                    // Match direct item or wildcard
//                    bool directMatch =
//                        IdHelper.CanonicalItemId(entry.ItemId)
//                            .Equals(IdHelper.CanonicalItemId(itemId), StringComparison.OrdinalIgnoreCase);

//                    bool wildcardMatch =
//                        entry.ItemId == "ALL_ITEMS (O)" &&
//                        (
//                            VendorHelper.ItemMatchesWildcard(itemId, entry) ||
//                            VendorHelper.EvaluatePerItemCondition(entry.PerItemCondition, itemId)
//                        );

//                    // Only check random pools if direct/wildcard failed
//                    bool randomPoolMatch = false;
//                    if (!directMatch && !wildcardMatch)
//                    {
//                        randomPoolMatch = VendorHelper.MatchesRandomPool(itemId, entry.ItemId);
//                    }


//                    if (!directMatch && !wildcardMatch && !randomPoolMatch)
//                        continue;
//                    ModEntry.Instance.Monitor.Log($"Direct {directMatch} Wildcard {wildcardMatch} Random {randomPoolMatch}", LogLevel.Info);

//                    var info = new PurchaseInfoData
//                    {
//                        VendorId = shopId,
//                        VendorName = VendorHelper.GetVendorName(shopId),
//                        Condition = entry.Condition
//                    };

//                    // Trade (non-gold)
//                    if (!string.IsNullOrEmpty(entry.TradeItemId))
//                    {
//                        info.TradeItemId = entry.TradeItemId;
//                        info.TradeAmount = entry.TradeItemAmount;
//                    }
//                    else
//                    {
//                        // PRICE CALCULATION BLOCK (restored)
//                        var item = ItemRegistry.Create(IdHelper.ToGameId(itemId));
//                        if (item != null)
//                        {
//                            var output = new ItemQueryResult(item);

//                            float basePrice = ShopBuilder.GetBasePrice(
//                                output,
//                                shop,
//                                entry,
//                                item,
//                                outOfSeasonPrice: false,
//                                useObjectDataPrice: entry.UseObjectDataPrice
//                            );

//                            float value = basePrice;

//                            // shop-level modifiers
//                            if (!entry.IgnoreShopPriceModifiers && shop.PriceModifiers != null)
//                            {
//                                value = Utility.ApplyQuantityModifiers(
//                                    value,
//                                    shop.PriceModifiers,
//                                    shop.PriceModifierMode,
//                                    null, null,
//                                    item,
//                                    null,
//                                    Utility.CreateDaySaveRandom()
//                                );
//                            }

//                            // item-level modifiers
//                            if (entry.PriceModifiers != null)
//                            {
//                                value = Utility.ApplyQuantityModifiers(
//                                    value,
//                                    entry.PriceModifiers,
//                                    entry.PriceModifierMode,
//                                    null, null,
//                                    item,
//                                    null,
//                                    Utility.CreateDaySaveRandom()
//                                );
//                            }

//                            info.GoldPrice = (int)value;
//                        }
//                    }

//                    // TRAVELING CART SPECIAL DETECTION
//                    bool isSpecial = false;

//                    if (VendorHelper.IsTravelingCart(shopId))
//                    {
//                        if (directMatch)
//                        {
//                            isSpecial = true;
//                            ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} matched direct entry.", LogLevel.Info);
//                        }

//                        if (wildcardMatch)
//                        {
//                            isSpecial = true;
//                            ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} matched wildcard entry.", LogLevel.Info);
//                        }

//                        if (randomPoolMatch)
//                        {
//                            isSpecial = true;
//                            ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} appears in Random ItemId pool: {entry.ItemId}", LogLevel.Info);
//                        }

//                        if (entry.PriceModifiers?.Count > 0)
//                        {
//                            isSpecial = true;
//                            ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} has price modifiers.", LogLevel.Info);
//                        }

//                        if (entry.PriceModifiers?.Any(m => m.RandomAmount?.Count > 0) == true)
//                        {
//                            isSpecial = true;
//                            ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} has RandomAmount pricing.", LogLevel.Info);
//                        }

//                        if (!string.IsNullOrEmpty(entry.PerItemCondition))
//                        {
//                            isSpecial = true;
//                            ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} has per-item conditions.", LogLevel.Info);
//                        }

//                        if (!isSpecial)
//                        {
//                            ModEntry.Instance.Monitor.Log($"[TC-NORMAL] {itemId} has a TC entry but no special rules.", LogLevel.Info);
//                        }
//                    }

//                    info.IsTravelingCartSpecial = isSpecial;

//                    results.Add(info);
//                }
//            }

//            return results;
//        }

//        private static bool GetGoldPrice(string itemId, ShopItemData entry)
//        {


//        }

//        private static bool SetTravelingCartSpecialFlag(string itemId, ShopItemData entry)
//        {


//        }



//    }
//}



// THE VERISON IN PROGRESS BEFORE TRYING TO MORE CLOSELY MATCH GAME LOGIC WITHOUT OVERRIDES
//using PlantingDay.Helpers;
//using SDVCommon.Helpers;
//using SDVData;
//using StardewModdingAPI;
//using StardewValley;
//using StardewValley.GameData.Shops;
//using StardewValley.Internal;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace PlantingDay.Helpers.SeedSource
//{
//    internal static class PurchaseDataBuilder
//    {
//        // ------------------------------------------------------------
//        // PUBLIC ENTRY POINT
//        // ------------------------------------------------------------
//        public static List<PurchaseInfoData> GetPurchaseInfo(string itemId)
//        {
//            var shops = Game1.content.Load<Dictionary<string, ShopData>>("Data/Shops");
//            return BuildPurchaseInfo(itemId, shops);
//        }

//        // ------------------------------------------------------------
//        // CORE EXTRACTION LOGIC
//        // ------------------------------------------------------------
//        public static List<PurchaseInfoData> BuildPurchaseInfo(
//            string itemId,
//            Dictionary<string, ShopData> shops)
//        {
//            var results = new List<PurchaseInfoData>();

//            foreach (var (shopId, shop) in shops)
//            {
//                foreach (var entry in shop.Items)
//                {
//                    // Match direct item or wildcard
//                    bool directMatch = IdHelper.CanonicalItemId(entry.ItemId) == itemId;
//                    bool wildcardMatch =
//                        entry.ItemId == "ALL_ITEMS (O)" &&
//                        (
//                            ItemMatchesWildcard(itemId, entry) ||   // context-tag based
//                            EvaluatePerItemCondition(entry.PerItemCondition, itemId) // ITEM_ID based
//                        );

//                    if (!directMatch && !wildcardMatch)
//                        continue;

//                    //Basic info
//                    var info = new PurchaseInfoData
//                    {
//                        VendorId = shopId,
//                        VendorName = VendorHelper.GetVendorName(shopId),
//                        Condition = entry.Condition
//                    };

//                    // Trade (non-gold)
//                    if (!string.IsNullOrEmpty(entry.TradeItemId))
//                    {
//                        info.TradeItemId = entry.TradeItemId;
//                        info.TradeAmount = entry.TradeItemAmount;
//                    }
//                    else
//                    {
//                        var item = ItemRegistry.Create(IdHelper.ToGameId(itemId));
//                        if (item != null)
//                        {
//                            var output = new ItemQueryResult(item);

//                            // First: calculate the real game price
//                            int calculatedPrice = ShopBuilder.GetBasePrice(
//                                output,
//                                shop,
//                                entry,
//                                item,
//                                outOfSeasonPrice: false,
//                                useObjectDataPrice: entry.UseObjectDataPrice
//                            );

//                        }
//                    }

//                    // Apply price modifiers
//                    if (entry.PriceModifiers != null)
//                    {
//                        int basePrice = info.GoldPrice ?? 0;

//                        foreach (var mod in entry.PriceModifiers)
//                        {
//                            switch (mod.Modification)
//                            {
//                                case StardewValley.GameData.QuantityModifier.ModificationType.Add:
//                                    info.GoldPrice += (int)mod.Amount;
//                                    break;

//                                case StardewValley.GameData.QuantityModifier.ModificationType.Subtract:
//                                    info.GoldPrice -= (int)mod.Amount;
//                                    break;

//                                case StardewValley.GameData.QuantityModifier.ModificationType.Multiply:
//                                    info.GoldPrice = (int)(basePrice * mod.Amount);
//                                    break;

//                                case StardewValley.GameData.QuantityModifier.ModificationType.Divide:
//                                    info.GoldPrice = (int)(basePrice / mod.Amount);
//                                    break;
//                            }
//                        }
//                    }

//                    // ------------------------------------------------------------
//                    // APPLY OVERRIDES (Vendor + Price)
//                    // ------------------------------------------------------------

//                    // Vendor override
//                    if (SeedOverrides.Vendor.TryGetValue((itemId, shopId), out var newVendor))
//                    {
//                        info.VendorId = newVendor;
//                        info.VendorName = VendorHelper.GetVendorName(newVendor);
//                    }

//                    // Price override
//                    if (SeedOverrides.Price.TryGetValue((itemId, shopId), out var overridePrice))
//                    {
//                        info.GoldPrice = overridePrice;
//                    }
//                    else
//                    {
//                        info.GoldPrice = calculatedPrice;
//                    }


//                    results.Add(info);
//                }
//            }

//            //
//            // Fallback: seeds with NO purchase options (Soybeans, Lentils, etc.)
//            //
//            if (results.Count == 0)
//            {
//                string vendorId = "";
//                int? price = null;

//                // Vendor override for fallback
//                if (SeedOverrides.Vendor.TryGetValue((itemId, ""), out var v))
//                    vendorId = v;

//                // Price override for fallback
//                if (SeedOverrides.Price.TryGetValue((itemId, ""), out var p))
//                    price = p;

//                // Only add fallback if we have at least one override
//                if (vendorId != "" || price.HasValue)
//                {
//                    results.Add(new PurchaseInfoData
//                    {
//                        VendorId = vendorId,
//                        VendorName = VendorHelper.GetVendorName(vendorId),
//                        GoldPrice = price,
//                        Condition = null
//                    });
//                }
//            }

//            return results;
//        }


//        // ------------------------------------------------------------
//        // WILDCARD MATCHING
//        // ------------------------------------------------------------
//        private static bool ItemMatchesWildcard(string itemId, ShopItemData entry)
//        {
//            if (string.IsNullOrWhiteSpace(entry.PerItemCondition))
//                return false;

//            // Only handle ITEM_CONTEXT_TAG rules here.
//            // If the condition has no context-tag rules, this wildcard matcher should not apply.
//            if (!entry.PerItemCondition.Contains("ITEM_CONTEXT_TAG", StringComparison.OrdinalIgnoreCase))
//                return false;

//            Item item = ItemRegistry.Create(IdHelper.ToGameId(itemId));
//            if (item == null)
//                return false;

//            var tags = item.GetContextTags();

//            var rules = entry.PerItemCondition
//                .Split(',')
//                .Select(r => r.Trim())
//                .Where(r => r.Length > 0);

//            foreach (var rule in rules)
//            {
//                bool negated = rule.StartsWith("!");
//                string cleanRule = negated ? rule[1..].Trim() : rule;

//                if (!cleanRule.StartsWith("ITEM_CONTEXT_TAG", StringComparison.OrdinalIgnoreCase))
//                    continue;

//                int idx = cleanRule.IndexOf("Target ", StringComparison.OrdinalIgnoreCase);
//                if (idx < 0)
//                    continue;

//                string requiredTag = cleanRule[(idx + "Target ".Length)..].Trim();
//                bool hasTag = tags.Contains(requiredTag);

//                if (negated && hasTag)
//                    return false;

//                if (!negated && !hasTag)
//                    return false;
//            }

//            return true;
//        }

//        // ------------------------------------------------------------
//        // CONDITION MATCHING
//        // ------------------------------------------------------------
//        private static bool EvaluatePerItemCondition(string? condition, string itemId)
//        {
//                if (string.IsNullOrEmpty(condition))
//                    return true;

//                // Example:
//                // ANY "ITEM_ID Target Cornucopia_Pansy" "ITEM_ID Target Cornucopia_Violet"
//                if (condition.StartsWith("ANY"))
//                {
//                    // Extract everything inside quotes
//                    var parts = condition.Split('"', StringSplitOptions.RemoveEmptyEntries);

//                    foreach (var part in parts)
//                    {
//                        if (part.Contains("ITEM_ID Target"))
//                        {
//                            var target = part.Split(' ', StringSplitOptions.RemoveEmptyEntries).Last();
//                        if (IdHelper.CanonicalItemId(itemId)
//                                .Equals(IdHelper.CanonicalItemId(target), StringComparison.OrdinalIgnoreCase))
//                            return true;
//                        }
//                    }

//                    return false;
//                }

//                return false;
//            }

//        }
//    }
