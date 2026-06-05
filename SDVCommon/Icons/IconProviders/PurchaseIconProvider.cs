using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using StardewModdingAPI;
using StardewValley;

namespace SDVCommon.Icons.IconProviders
{
    public class PurchaseIconProvider : IIconProvider
    {
        public bool CanHandle(string id)
        {
            return id.StartsWith("item:", System.StringComparison.OrdinalIgnoreCase);
        }

        public Icon? LoadIcon(string id)
        {
            try
            {
                string ItemId = id["item:".Length..];

                if (string.IsNullOrWhiteSpace(ItemId))
                    return null;

                Item item = ItemRegistry.Create(ItemId);
                if (item == null)
                    return null;

                var data = ItemRegistry.GetData(item.QualifiedItemId);
                if (data == null)
                    return null;

                Texture2D texture = data.GetTexture();
                Rectangle source = data.GetSourceRect();

                return new Icon(texture, source, size: source.Width, scale: 2f);
            }
            catch
            {
                return null;
            }
        }
    }
}
