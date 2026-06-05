using SDVCommon.Helpers;
using SDVCommon.GameData;
using SDVData;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using static SDVData.PurchaseInfoData;

namespace SDVCommon.Models.Builders
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
                        IdHelper.ToItemId(entry.ItemId)
                            .Equals(IdHelper.ToItemId(itemId), StringComparison.OrdinalIgnoreCase);

                    bool wildcardMatch =
                        entry.ItemId == "ALL_ITEMS (O)" &&
                        (
                            Vendor.ItemMatchesWildcard(itemId, entry) ||
                            Vendor.EvaluatePerItemCondition(entry.PerItemCondition, itemId)
                        );

                    bool randomPoolMatch = false;
                    if (!directMatch && !wildcardMatch)
                        randomPoolMatch = Vendor.MatchesRandomPool(itemId, entry.ItemId);

                    if (!directMatch && !wildcardMatch && !randomPoolMatch)
                        continue;

                    // -----------------------------
                    // CREATE PURCHASE INFO ENTRY
                    // -----------------------------
                    var info = new PurchaseInfoData
                    {
                        VendorId = shopId,
                        VendorName = Vendor.GetVendorName(shopId),
                        Condition = entry.Condition,
                        Type = Vendor.GetVendorType(shopId),
                    };


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
            var item = ItemRegistry.Create(IdHelper.ToItemId(itemId));
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

    }
}

