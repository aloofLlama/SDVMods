using SDVCommon.Compatibility;
using SDVCommon.GameData.Dictionaries;
using SDVCommon.Helpers;
using SDVCommon.Services;
using SDVData;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.GameData.Objects;
using System.Runtime.InteropServices;


namespace SDVCommon.GameData
{
    public class GameObject
    {
        // Accepts both qualified and unqualified IDs
        public static StardewValley.Object? GetObjectInstance(string id, int quality = 0)
        {
            string unqualifiedId = IdHelper.ToUnqualifiedItemId(id);

            // Ensure the object exists in objectData
            if (!Game1.objectData.ContainsKey(unqualifiedId))
                return null;

            // Create a real SObject instance
            return new StardewValley.Object(unqualifiedId, 1, false, -1, quality);
        }

        public static ObjectInfo? GetObjectInfo(string qId)
        {
            string unqualifiedId = IdHelper.ToUnqualifiedItemId(qId);
            var obj = GetObjectInstance(qId);

            if (!Game1.objectData.TryGetValue(unqualifiedId, out var objData) ||
                obj == null)
            {
                    return null;
            }
            

            return new ObjectInfo
            {
                QId = qId,
                DisplayName = obj.DisplayName,
                //Description = objData.Description,
                Price = objData.Price,
                Category = objData.Category,
                Edibility = objData.Edibility,
                Type = objData.Type,
                ContextTags = objData.ContextTags?.ToList()
            };
        }

        // Quality is 0 for normal, 1 for silver, 2 for gold, 4 for iridium
        public static int GetSellPrice(string qId, int quality = 0)
        {
            var obj = GameObject.GetObjectInstance(qId, quality);
            return obj?.sellToStorePrice() ?? 0;
        }


#if DEBUG
        // Used for debugging purposes. Displays all the ObjectInfo data for a qId
        public static void DumpObjectInfo(string qId)
        {
            var objInfo = GameObject.GetObjectInfo(qId);

            if (objInfo != null)
            {
                SDVCommonLog.Log(   // is flagged as #if DEBUG
                    $"QId: {objInfo.QId}\n" +
                    $"DisplayName: {objInfo.DisplayName}\n" +
                    $"Price: {objInfo.Price}\n" +
                    $"Category: {objInfo.Category}\n" +
                    $"Edibility: {objInfo.Edibility}\n" +
                    $"Type: {objInfo.Type}\n" +
                    $"Tags: {string.Join(", ", objInfo.ContextTags ?? new List<string>())}",
                    LogHelper.InfoOrTrace);
            }
            else
                SDVCommonLog.Log("null", LogHelper.InfoOrTrace); // is flagged as #if DEBUG
        }
#endif

    }
}
