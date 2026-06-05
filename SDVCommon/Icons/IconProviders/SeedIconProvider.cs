using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon.Icons;
using StardewModdingAPI;
using StardewValley;
using SDVCommon;

namespace SDVCommon.Icons.IconProviders
{
    public class SeedIconProvider : IIconProvider
    {
        public bool CanHandle(string id)
        {

            return id.StartsWith("seed:", StringComparison.OrdinalIgnoreCase);
        
        }

        public Icon? LoadIcon(string id)
        {
            string seedId = id.Substring("seed:".Length);

            // Create the item
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
