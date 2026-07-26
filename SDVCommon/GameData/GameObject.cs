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
            if (!Game1.objectData.TryGetValue(objectId, out var obj))
            {
                    return null;
            }

            var objInstance = new StardewValley.Object(objectId, 1);

            return new ItemInfo
            {
                Id = objectId,
                DisplayName = objInstance.DisplayName,
                //Description = obj.Description,
                //Price = obj.Price,
                Category = obj.Category,
                //Edibility = obj.Edibility,
                Type = obj.Type,
                ContextTags = obj.ContextTags?.ToList()
            };
        }

    }
}
