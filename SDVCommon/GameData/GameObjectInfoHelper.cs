using SDVData;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Text;


namespace SDVCommon.GameData
{
    public class GameObjectInfoHelper
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
                Type = obj.Type
            };
        }
    }
}
