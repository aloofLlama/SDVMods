using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay.Helpers.Icons;
using StardewModdingAPI;
using StardewValley;

namespace PlantingDay.Helpers
{
    public class HarvestIconProvider : IIconProvider
    {
        public bool CanHandle(string id)
        {
            ModEntry.Instance.Monitor.Log(
                $"Provider {GetType().Name} checking '{id}'",
                LogLevel.Warn
            );

            return id.StartsWith("harvest:", StringComparison.OrdinalIgnoreCase);
        }

        public Icon? LoadIcon(string id)
        {
            // Extract canonical item ID
            string harvestId = id.Substring("harvest:".Length);

            var item = ItemRegistry.Create(harvestId);
            if (item == null)
                return null;

            var tex = IconRenderer.RenderItemIcon(item, 32);
            return new Icon(
                tex,
                new Rectangle(0, 0, tex.Width, tex.Height),
                size: tex.Width,
                scale: 1f
            );
        }
    }
}
