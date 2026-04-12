using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Helpers.SeedSource
{
    internal static class MonsterDropLoader
    {
        private static Dictionary<string, List<(int itemId, float chance)>>? _monsterDropTable;

        public static void Initialize()
        {
            _monsterDropTable = new();

            var monsters = Game1.content.Load<Dictionary<string, string>>("Data/Monsters");

            foreach (var (monsterName, raw) in monsters)
            {
                string[] fields = raw.Split('/');

                // Field 6 = drop list
                if (fields.Length <= 6)
                    continue;

                string dropField = fields[6];
                var parts = dropField.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

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

        /// <summary>
        /// Return all monsters that drop the given seed item.
        /// Produces MonsterDropInfoData entries for PlantInfoData.
        /// </summary>
        public static List<SDVData.MonsterDropInfoData> GetDropsForItem(string seedId)
        {
            var result = new List<SDVData.MonsterDropInfoData>();

            if (_monsterDropTable is null)
                return result;

            if (!int.TryParse(seedId, out int itemId))
                return result;

            foreach (var (monsterName, drops) in _monsterDropTable)
            {
                foreach (var (dropId, chance) in drops)
                {
                    if (dropId == itemId)
                    {
                        result.Add(new SDVData.MonsterDropInfoData
                        {
                            MonsterName = monsterName,
                            DropChance = chance,
                            // IconRef assigned later in PlantDatabase.Initialize()
                        });
                    }
                }
            }

            return result;
        }
    }
}
