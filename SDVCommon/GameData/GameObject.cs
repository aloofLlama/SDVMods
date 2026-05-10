using SDVCommon.Compatibility;
using SDVCommon.Helpers;
using SDVData;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.GameData.Objects;


namespace SDVCommon.GameData
{
    public class GameObject
    {
        public static ItemInfo? FromObject(string objectId)
        {

            // 1) Try raw key first (works for modded IDs and numeric vanilla)
            if (!Game1.objectData.TryGetValue(objectId, out var obj))
            {
                // 2) Fallback: extract numeric ID for "(O)638" / "O:638" / "638"
                string numeric = new(objectId.Where(char.IsDigit).ToArray());

                if (string.IsNullOrEmpty(numeric) ||
                    !Game1.objectData.TryGetValue(numeric, out obj))
                {
                    return null;
                }
            }

            return new ItemInfo
            {
                Id = objectId,
                Name = obj.Name,
                Description = obj.Description,
                Price = obj.Price,
                Category = obj.Category,
                Edibility = obj.Edibility,
                Type = obj.Type,
                ContextTags = obj.ContextTags?.ToList()
            };
        }

        public static StardewValley.Object? GetObjectFromId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            // Normalize: remove (O) prefix, JA prefixes, etc.
            string canonical = IdHelper.CanonicalItemId(id);

            // If numeric → vanilla object
            if (int.TryParse(canonical, out int parentSheetIndex))
                return new StardewValley.Object(parentSheetIndex.ToString(), 1);

            // Otherwise → modded object (JA, DGA, CP, etc.)
            Item? item = ItemRegistry.Create(canonical);
            return item as StardewValley.Object;
        }




    }
}
