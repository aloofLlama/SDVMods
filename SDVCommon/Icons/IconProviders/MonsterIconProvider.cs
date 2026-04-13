using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace SDVCommon.Icons.IconProviders
{
    public class MonsterIconProvider : IIconProvider
    {
        public bool CanHandle(string id)
        {
            ModEntry.Instance.Monitor.Log($"[MonsterIconProvider] CanHandle? id='{id}'", LogLevel.Warn);
            ModEntry.Instance.Monitor.Log(
    $"[MonsterIconProvider] CanHandle? id='{id}' (prefix check: {id.StartsWith("monster:", StringComparison.OrdinalIgnoreCase)})",
    LogLevel.Warn
);


            // Accept raw monster names directly
            return Game1.content.Load<Dictionary<string, string>>("Data/Monsters")
                .ContainsKey(id);
        }

        public Icon? LoadIcon(string id)
        {
            ModEntry.Instance.Monitor.Log($"[MonsterIconProvider] LoadIcon called with id='{id}'", LogLevel.Warn);

            try
            {
                var monsters = Game1.content.Load<Dictionary<string, string>>("Data/Monsters");

                // id IS the monster name
                if (!monsters.TryGetValue(id, out string? raw))
                    return null;

                string[] fields = raw.Split('/');
                string texturePath = fields[0];
                int frameWidth = int.Parse(fields[1]);
                int frameHeight = int.Parse(fields[2]);

                Texture2D tex = Game1.content.Load<Texture2D>(texturePath);
                Rectangle source = new Rectangle(0, 0, frameWidth, frameHeight);

                return new Icon(tex, source, frameWidth, scale: 2f);
            }
            catch
            {
                return null;
            }
        }
    }
}
