using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.Helpers
{
    internal static class IconRenderer_Dynamic
    {
        // Cache to avoid reloading textures repeatedly
        private static readonly Dictionary<string, IconRef?> MonsterIconCache = new();
        private static readonly Dictionary<string, IconRef?> CurrencyIconCache = new();

        // ------------------------------------------------------------
        // PUBLIC API
        // ------------------------------------------------------------
        public static IconRef? GetMonsterIcon(string monsterName)
        {
            if (MonsterIconCache.TryGetValue(monsterName, out var cached))
                return cached;

            var icon = LoadMonsterIcon(monsterName);
            MonsterIconCache[monsterName] = icon;
            return icon;
        }

        public static IconRef? GetCurrencyIcon(string tradeItemId)
        {
            if (CurrencyIconCache.TryGetValue(tradeItemId, out var cached))
                return cached;

            var icon = LoadCurrencyIcon(tradeItemId);
            CurrencyIconCache[tradeItemId] = icon;
            return icon;
        }

        // ------------------------------------------------------------
        // INTERNAL LOADERS
        // ------------------------------------------------------------
        private static IconRef? LoadMonsterIcon(string monsterName)
        {
            try
            {
                var monsters = Game1.content.Load<Dictionary<string, string>>("Data/Monsters");
                if (!monsters.TryGetValue(monsterName, out string? raw))
                    return null;

                string[] fields = raw.Split('/');
                string texturePath = fields[0];
                int frameWidth = int.Parse(fields[1]);
                int frameHeight = int.Parse(fields[2]);

                Texture2D tex = Game1.content.Load<Texture2D>(texturePath);
                Rectangle source = new Rectangle(0, 0, frameWidth, frameHeight);

                return new IconRef(tex, source, frameWidth, scale: 2f);
            }
            catch
            {
                return null;
            }
        }

        private static IconRef? LoadCurrencyIcon(string tradeItemId)
        {
            if (string.IsNullOrWhiteSpace(tradeItemId))
                return null;

            Item item = ItemRegistry.Create(tradeItemId);
            if (item == null)
                return null;

            var data = ItemRegistry.GetData(item.QualifiedItemId);
            if (data == null)
                return null;

            Texture2D texture = data.GetTexture();
            Rectangle source = data.GetSourceRect();

            return new IconRef(texture, source, size: source.Width, scale: 2f);
        }
    }
}
