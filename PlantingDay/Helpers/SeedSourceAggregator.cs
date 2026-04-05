using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PlantingDay.Models;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Helpers
{
    public static class SeedSourceAggregator
    {
        public static List<object> BuildFullSourceList(PlantInfo plant)
        {
            var list = new List<object>();

            // 1. Get raw vendor entries (collapsed, filtered)
            var vendors = VendorListBuilder.Build(plant);

            // 2. Split vendors into categories
            var pierre = vendors.Where(v => VendorHelper.IsPierre(v)).ToList();
            var goldVendors = vendors.Where(v => v.GoldPrice.HasValue && !VendorHelper.IsPierre(v) && !v.IsNightMarket).ToList();
            var tradeVendors = vendors.Where(v => v.TradeAmount > 0 && !v.IsNightMarket).ToList();
            var nightMarket = vendors.Where(v => v.IsNightMarket).ToList();

            // 3. Sort each category alphabetically
            goldVendors.Sort((a, b) => string.Compare(a.VendorName, b.VendorName, StringComparison.OrdinalIgnoreCase));
            tradeVendors.Sort((a, b) => string.Compare(a.VendorName, b.VendorName, StringComparison.OrdinalIgnoreCase));
            nightMarket.Sort((a, b) => string.Compare(a.VendorName, b.VendorName, StringComparison.OrdinalIgnoreCase));

            // 4. Add in the correct order
            list.AddRange(pierre);        // 0. Pierre
            list.AddRange(goldVendors);   // 1. Gold vendors
            list.AddRange(tradeVendors);  // 2. Trade vendors
            list.AddRange(plant.MonsterDrops); // 3. Monster drops
            list.AddRange(nightMarket);   // 4. Night Market

            return list;
        }

        // Run once during load
        public static void AddSeedSourcesToPlant(PlantInfo plant)
        {
            // 1. Vendor purchase info referenced from game and saved to plant database
            plant.PurchaseOptions = VendorHelper.GetPurchaseInfo(plant.SeedId);

            // 2. Monster drops (already taken from game during save load) and saved to plant database
            plant.MonsterDrops = MonsterDropLoader.GetDropsForItem(plant.SeedId);

            // 3. Dynamic icons (currency + monster) save to plant database
            foreach (var p in plant.PurchaseOptions)
            {
                if (!string.IsNullOrEmpty(p.TradeItemId))
                    p.CurrencyIconRef = IconRenderer_Dynamic.GetCurrencyIcon(p.TradeItemId);
            }

            foreach (var d in plant.MonsterDrops)
            {
                if (!string.IsNullOrEmpty(d.MonsterName))
                    d.MonsterIconRef = IconRenderer_Dynamic.GetMonsterIcon(d.MonsterName);
            }
        }

        //-------------
        // Monster Drops
        //--------------

        public static class MonsterDropLoader
        {
            private static Dictionary<string, List<(int itemId, float chance)>>? _monsterDropTable;

            public static void Initialize()
            {
                _monsterDropTable = new();

                var monsters = Game1.content.Load<Dictionary<string, string>>("Data/Monsters");

                foreach (var (monsterName, raw) in monsters)
                {
                    string[] fields = raw.Split('/');

                    if (fields.Length <= 6)
                        continue;

                    string dropField = fields[6];
                    var parts = dropField.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    var drops = new List<(int, float)>();

                    for (int i = 0; i < parts.Length - 1; i += 2)
                    {
                        if (int.TryParse(parts[i], out int id) &&
                            float.TryParse(parts[i + 1], out float chance))
                        {
                            drops.Add((id, chance));
                        }
                    }

                    if (drops.Count > 0)
                        _monsterDropTable[monsterName] = drops;
                }
            }

            // Save monster drop info into Plant Database.
            // (Game droptable already created during load)
            public static List<MonsterDropInfo> GetDropsForItem(string seedId)
            {
                if (_monsterDropTable is null)
                    return new List<MonsterDropInfo>();

                if (!int.TryParse(seedId, out int itemId))
                    return new List<MonsterDropInfo>();

                var result = new List<MonsterDropInfo>();

                foreach (var (monsterName, drops) in _monsterDropTable)
                {
                    foreach (var (dropId, chance) in drops)
                    {
                        if (dropId == itemId)
                        {
                            result.Add(new MonsterDropInfo
                            {
                                MonsterName = monsterName,
                                Chance = chance,
                                MonsterIconRef = IconRenderer_Dynamic.GetMonsterIcon(monsterName)
                            });
                        }
                    }
                }

                return result;
            }

        }



    }
}
