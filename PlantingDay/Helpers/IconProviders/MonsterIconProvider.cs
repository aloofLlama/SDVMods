using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay.Helpers.Icons;
using StardewValley;
using System.Collections.Generic;

namespace PlantingDay.Helpers
{
    public class MonsterIconProvider : IIconProvider
    {
        public bool CanHandle(string id)
        {
            return id.StartsWith("monster:", System.StringComparison.OrdinalIgnoreCase);
        }

        public Icon? LoadIcon(string id)
        {
            // Extract monster name
            string monsterName = id.Substring("monster:".Length);

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

                return new Icon(tex, source, frameWidth, scale: 2f);
            }
            catch
            {
                return null;
            }
        }
    }
}
