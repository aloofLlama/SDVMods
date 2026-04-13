using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay;
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
            ModEntry.Instance.Monitor.Log($"[PurchaseIconProvider] CanHandle? id='{id}'", LogLevel.Warn);

            return id.StartsWith("item:", System.StringComparison.OrdinalIgnoreCase);
        }

        public Icon? LoadIcon(string id)
        {
            ModEntry.Instance.Monitor.Log($"[PurchaseIconProvider] LoadIcon called with id='{id}'", LogLevel.Warn);

            try
            {
                // Extract canonical item ID
                string canonicalId = id["item:".Length..];

                if (string.IsNullOrWhiteSpace(canonicalId))
                    return null;

                // Convert canonical → game ID
                string gameId = IdHelper.ToGameId(canonicalId);

                Item item = ItemRegistry.Create(gameId);
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
