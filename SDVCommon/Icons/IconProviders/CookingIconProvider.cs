using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace SDVCommon.Icons.IconProviders
{
    public class CookingIconProvider : IIconProvider
    {
        public bool CanHandle(string id)
        {
            return id.StartsWith("cooking:", StringComparison.OrdinalIgnoreCase);
        }

        public Icon? LoadIcon(string id)
        {
            string itemId = id.Substring("cooking:".Length);

            var item = ItemRegistry.Create(itemId);
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


