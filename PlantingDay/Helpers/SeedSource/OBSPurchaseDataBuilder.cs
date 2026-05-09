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
//using System.Numerics;
//using static SDVData.PurchaseInfoData;

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
//                    // -----------------------------
//                    // Is the shop entry a seed
//                    // -----------------------------
//                    bool directMatch =
//                        IdHelper.CanonicalItemId(entry.ItemId)
//                            .Equals(IdHelper.CanonicalItemId(itemId), StringComparison.OrdinalIgnoreCase);

//                    bool wildcardMatch =
//                        entry.ItemId == "ALL_ITEMS (O)" &&
//                        (
//                            OBSVendorHelper.ItemMatchesWildcard(itemId, entry) ||
//                            OBSVendorHelper.EvaluatePerItemCondition(entry.PerItemCondition, itemId)
//                        );

//                    bool randomPoolMatch = false;
//                    if (!directMatch && !wildcardMatch)
//                        randomPoolMatch = OBSVendorHelper.MatchesRandomPool(itemId, entry.ItemId);

//                    if (!directMatch && !wildcardMatch && !randomPoolMatch)
//                        continue;

//                    // -----------------------------
//                    // CREATE PURCHASE INFO ENTRY
//                    // -----------------------------
//                    var info = new PurchaseInfoData
//                    {
//                        VendorId = shopId,
//                        VendorName = OBSVendorHelper.GetVendorName(shopId),
//                        Condition = entry.Condition,

//                        //IsNightMarket = VendorHelper.IsNightMarket(shopId),
//                        //IsTravelingCartSpecial = false // set below
//                    };
//                    info.Type = OBSVendorHelper.GetVendorType(shopId);


//                    // -----------------------------
//                    // PRICE OR TRADE
//                    // -----------------------------
//                    if (!string.IsNullOrEmpty(entry.TradeItemId))
//                    {
//                        info.TradeItemId = entry.TradeItemId;
//                        info.TradeAmount = entry.TradeItemAmount;
//                    }
//                    else
//                    {
//                        info.GoldPrice = GetGoldPrice(itemId, shop, entry);
//                    }

//                    results.Add(info);
//                }
//            }

//            return results;
//        }

//        // --------------------------------------
//        // PRICE CALCULATION
//        // --------------------------------------
//        private static int? GetGoldPrice(string itemId, ShopData shop, ShopItemData entry)
//        {
//            var item = ItemRegistry.Create(IdHelper.ToGameId(itemId));
//            if (item == null)
//                return null;

//            var output = new ItemQueryResult(item);

//            float value = ShopBuilder.GetBasePrice(
//                output,
//                shop,
//                entry,
//                item,
//                outOfSeasonPrice: false,
//                useObjectDataPrice: entry.UseObjectDataPrice
//            );

//            // shop-level modifiers
//            if (!entry.IgnoreShopPriceModifiers && shop.PriceModifiers != null)
//            {
//                value = Utility.ApplyQuantityModifiers(
//                    value,
//                    shop.PriceModifiers,
//                    shop.PriceModifierMode,
//                    null, null,
//                    item,
//                    null,
//                    Utility.CreateDaySaveRandom()
//                );
//            }

//            // item-level modifiers
//            if (entry.PriceModifiers != null)
//            {
//                value = Utility.ApplyQuantityModifiers(
//                    value,
//                    entry.PriceModifiers,
//                    entry.PriceModifierMode,
//                    null, null,
//                    item,
//                    null,
//                    Utility.CreateDaySaveRandom()
//                );
//            }

//            return (int)value;
//        }

//        // --------------------------------------
//        // TRAVELING CART SPECIAL FLAG
//        // --------------------------------------
//        private static bool SetTravelingCartSpecialFlag(
//            string itemId,
//            string shopId,
//            ShopItemData entry,
//            bool directMatch,
//            bool wildcardMatch,
//            bool randomPoolMatch)
//        {
//            if (OBSVendorHelper.GetVendorType(shopId) != VendorType.TravelingCart)
//                return false;

//            bool isSpecial = false;

//            if (directMatch)
//            {
//                isSpecial = true;
//                ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} matched direct entry.", LogLevel.Info);
//            }

//            if (wildcardMatch)
//            {
//                isSpecial = true;
//                ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} matched wildcard entry.", LogLevel.Info);
//            }

//            if (randomPoolMatch)
//            {
//                isSpecial = true;
//                ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} appears in Random ItemId pool: {entry.ItemId}", LogLevel.Info);
//            }

//            if (entry.PriceModifiers?.Count > 0)
//            {
//                isSpecial = true;
//                ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} has price modifiers.", LogLevel.Info);
//            }

//            if (entry.PriceModifiers?.Any(m => m.RandomAmount?.Count > 0) == true)
//            {
//                isSpecial = true;
//                ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} has RandomAmount pricing.", LogLevel.Info);
//            }

//            if (!string.IsNullOrEmpty(entry.PerItemCondition))
//            {
//                isSpecial = true;
//                ModEntry.Instance.Monitor.Log($"[TC-SPECIAL] {itemId} has per-item conditions.", LogLevel.Info);
//            }

//            if (!isSpecial)
//            {
//                ModEntry.Instance.Monitor.Log($"[TC-NORMAL] {itemId} has a TC entry but no special rules.", LogLevel.Info);
//            }

//            return isSpecial;
//        }
//    }
//}

