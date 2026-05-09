using SDVCommon.Compatibility;
using SDVData;
using StardewValley;

namespace SDVCommon.Models.Builders
{
    internal static class MonsterDropBuilder
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
        public static List<MonsterDropInfoData> GetDropsForItem(string seedId)
        {
            var result = new List<MonsterDropInfoData>();

            if (_monsterDropTable is null)
                return result;

            if (!int.TryParse(seedId, out int itemId))
                return result;

            foreach (var (monsterName, drops) in _monsterDropTable)
            {
                // collect all chances for this monster + item
                var chances = drops
                    .Where(d => d.itemId == itemId)
                    .Select(d => d.chance)
                    .ToList();

                if (chances.Count == 0)
                    continue;

                // combine independent probabilities
                float effectiveChance = 1f;
                foreach (var c in chances)
                    effectiveChance *= (1f - c);

                effectiveChance = 1f - effectiveChance;

                result.Add(new MonsterDropInfoData
                {
                    MonsterName = monsterName,
                    DropChance = effectiveChance
                });
            }

            // Apply overrides
            if (DataOverrides.MonsterDrops.TryGetValue(seedId, out var overrideList))
            {

                return overrideList
                    .Select(m => new MonsterDropInfoData
                    {
                        MonsterName = m.Monster,
                        DropChance = (float)m.Chance
                    })
                    .ToList();

            }

            return result;
        }
    }
}
