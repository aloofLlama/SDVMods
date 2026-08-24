using SDVData;
using SDVCommon.GameData;

namespace HarvestHelper.Helpers
{
    //TODO: PART OF DETERMINING IF SEED MAKER IS ECONOMICAL
    public static class EconomicsHelper
    {
        public static int? GetMinSeedPriceFromMainVendors(PlantInfoData plant)
        {
            if (plant == null || plant.PurchaseOptions == null)
                return null;

            // Allowed mainstream vendors
            var allowed = new HashSet<VendorType>
            {
                VendorType.Pierre,
                VendorType.Oasis,
                VendorType.Marnie,
                //VendorType.Ari, //Sunberry
                //VendorType.Jumana //Sunberry
            };


            // Filter to allowed vendors with a gold price
            var prices = plant.PurchaseOptions
                .Where(p => allowed.Contains(p.Type) && p.GoldPrice.HasValue)
                .Select(p => p.GoldPrice!.Value);

            // Return min or null if none
            return prices.Any() ? prices.Min() : (int?)null;
        }

        public static int GetCookingByCategoryThresholdPrice(int category)
        {
            switch (category)
            {
                case -4: // fish
                    return ModEntry.ModConfig.AnyFishPriceThreshold == -10
                        ? GameObject.GetSellPrice("(O)721", 1) // Snail, silver
                        : ModEntry.ModConfig.AnyFishPriceThreshold;

                case -5: // egg
                    return ModEntry.ModConfig.AnyEggPriceThreshold == -10
                        ? GameObject.GetSellPrice("(O)182", 4) // Large Egg, iridium
                        : ModEntry.ModConfig.AnyEggPriceThreshold;

                case -6: // milk
                    return ModEntry.ModConfig.AnyMilkPriceThreshold == -10
                        ? GameObject.GetSellPrice("(O)186", 4) // Large Milk, iridium
                        : ModEntry.ModConfig.AnyMilkPriceThreshold;

                case -75: // vegetable
                    return ModEntry.ModConfig.AnyVeggiePriceThreshold == -10
                        ? GameObject.GetSellPrice("(O)272", 2) // Eggplant, gold
                        : ModEntry.ModConfig.AnyVeggiePriceThreshold;

                case -79: // fruit
                    return ModEntry.ModConfig.AnyFruitPriceThreshold == -10
                        ? GameObject.GetSellPrice("(O)282", 2) // Cranberry, gold
                        : ModEntry.ModConfig.AnyFruitPriceThreshold;
            }

            return 0;
        }
    }
}
