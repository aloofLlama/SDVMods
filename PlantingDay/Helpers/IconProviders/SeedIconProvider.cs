using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay.Helpers.Icons;
using StardewModdingAPI;
using StardewValley;

namespace PlantingDay.Helpers
{
    public class SeedIconProvider : IIconProvider
    {
        public bool CanHandle(string id)
        {
            ModEntry.Instance.Monitor.Log($"{GetType().Name}.CanHandle('{id}')", LogLevel.Warn);

            return id.StartsWith("seed:", StringComparison.OrdinalIgnoreCase);
        
        }

        public Icon? LoadIcon(string id)
        {
            // Extract canonical item ID
            string seedId = id.Substring("seed:".Length);

            // Create the item from the canonical ID
            var item = ItemRegistry.Create(seedId);
            if (item == null)
                return null;

            // Render the icon
            var tex = IconRenderer.RenderItemIcon(item, 16);
            return new Icon(
                tex,
                new Rectangle(0, 0, tex.Width, tex.Height),
                size: tex.Width,
                scale: 2f
            );
        }
    }
}
