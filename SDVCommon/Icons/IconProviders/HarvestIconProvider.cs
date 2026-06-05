using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon.Icons;
using StardewModdingAPI;
using StardewValley;

namespace SDVCommon.Icons.IconProviders
{
    public class HarvestIconProvider : IIconProvider
    {
        public bool CanHandle(string id)
        {

            return id.StartsWith("harvest:", StringComparison.OrdinalIgnoreCase);
        }

        public Icon? LoadIcon(string id)
        {
            string harvestId = id.Substring("harvest:".Length);

            var item = ItemRegistry.Create(harvestId);
            if (item == null)
                return null;

            var tex = SDVCommon.Icons.IconRenderer.RenderItemIcon(item, 32);
            return new Icon(
                tex,
                new Rectangle(0, 0, tex.Width, tex.Height),
                size: tex.Width,
                scale: 1f
            );
        }
    }
}
