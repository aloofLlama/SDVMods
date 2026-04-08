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

            // 1. Get fully processed vendor list (already collapsed + sorted)
            var vendors = VendorListBuilder.Build(plant);

            // 2. Split Night Market out
            var nightMarket = vendors.Where(v => VendorHelper.IsNightMarket(v)).ToList();
            var nonNightVendors = vendors.Where(v => !VendorHelper.IsNightMarket(v)).ToList();

            // 3. Add in your desired order:
            //    1. Most vendors (non-Night Market)
            list.AddRange(nonNightVendors);

            //    2. Monster drops
            list.AddRange(plant.MonsterDrops);

            //    3. Night Market (always last)
            list.AddRange(nightMarket);

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
