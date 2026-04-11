using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay.Helpers.Icons;
using StardewValley;

namespace PlantingDay.Helpers
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
                // Extract canonical item ID
                string tradeItemId = id.Substring("item:".Length);

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

                return new Icon(texture, source, size: source.Width, scale: 2f);
            }
            catch
            {
                return null;
            }
        }
    }
}
