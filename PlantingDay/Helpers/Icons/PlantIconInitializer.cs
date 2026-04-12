using PlantingDay.Models.Wrappers;
using SDVCommon.Icons;

namespace PlantingDay.Helpers.Icons
{
    internal static class PlantIconInitializer
    {
        public static void InitializeIcons(PlantInfo plant)
        {
            // Seed
            plant.Runtime.SeedIcon = IconRegistry.GetIcon($"seed:{plant.Data.SeedId}");
            //plant.Runtime.HarvestIcon = IconRegistry.GetIcon($"harvest:{plant.Data.HarvestId}");

            // Vendor currency icons
            foreach (var vendor in plant.PurchaseOptions)
            {
                if (!string.IsNullOrEmpty(vendor.Data.TradeItemId))
                    vendor.CurrencyIcon = IconRegistry.GetIcon($"item:{vendor.Data.TradeItemId}");
            }

            // Monster icons
            foreach (var drop in plant.MonsterDrops)
            {
                drop.MonsterIcon = IconRegistry.GetIcon($"monster:{drop.Data.MonsterName}");
            }
        }
    }
}
