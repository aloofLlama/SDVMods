using PlantingDay.Models.Wrappers;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using StardewModdingAPI;

namespace PlantingDay.Helpers.Icons
{
    internal static class PlantIconInitializer
    {
        public static void InitializeIcons(PlantInfo plant)
        {
            // Seed
            plant.Runtime.SeedIcon = IconRegistry.GetIcon($"seed:{plant.Data.SeedId}");

            // Vendor currency icons
            foreach (var vendor in plant.PurchaseOptions)
            {

                if (!string.IsNullOrEmpty(vendor.Data.TradeItemId))
                {
                    ModEntry.Instance.Monitor.Log($"Vendor: {vendor.Data.TradeItemId}", LogLevel.Info);
                    ModEntry.Instance.Monitor.Log($"Vendor: {IdHelper.CanonicalItemId(vendor.Data.TradeItemId)}", LogLevel.Info);


                    vendor.CurrencyIcon = IconRegistry.GetIcon($"item:{IdHelper.CanonicalItemId(vendor.Data.TradeItemId)}");
                }
            }

            // Monster icons
            foreach (var drop in plant.MonsterDrops)
            {
                drop.MonsterIcon = IconRegistry.GetIcon($"monster:{IdHelper.CanonicalItemId(drop.Data.MonsterName)}");
            }
        }
    }
}
