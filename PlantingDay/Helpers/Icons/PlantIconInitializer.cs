using PlantingDay.Models.Wrappers;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using StardewModdingAPI;

namespace PlantingDay.Helpers.Icons
{
    internal static class PlantIconInitializer
    {
        private static readonly Dictionary<string, IconKey> MonsterIconMap = new()
        {
            ["Dust Spirit"] = IconKey.DustSprite,
            ["Grub"] = IconKey.Grub,
            ["Magma Duggy"] = IconKey.MagmaDuggy,
            ["Hot Head"] = IconKey.HotHead,
            ["Mummy"] = IconKey.Mummy,
            ["Serpent"] = IconKey.Serpent,
            ["Purple Slime"] = IconKey.PurpleSlime

        };

        public static void InitializeIcons(PlantInfo plant)
        {
            // Seed
            plant.Runtime.SeedIcon = IconRegistry.GetIcon($"seed:{plant.Data.SeedId}");

            // Vendor currency icons
            foreach (var vendor in plant.PurchaseOptions)
            {

                if (!string.IsNullOrEmpty(vendor.Data.TradeItemId))
                {
                    //ModEntry.Instance.Monitor.Log($"Vendor: {vendor.Data.TradeItemId}", LogLevel.Info);
                    //ModEntry.Instance.Monitor.Log($"Vendor: {IdHelper.CanonicalItemId(vendor.Data.TradeItemId)}", LogLevel.Info);


                    vendor.CurrencyIcon = IconRegistry.GetIcon($"item:{IdHelper.CanonicalItemId(vendor.Data.TradeItemId)}");
                }
            }

            // Monster icons
            foreach (var drop in plant.MonsterDrops)
            {
                var name = drop.Data.MonsterName;
                if (name is not null && MonsterIconMap.TryGetValue(name, out var key))
                {
                    drop.MonsterIcon = TooltipIcons.Get(key);
                }
            }
        }
    }
}
